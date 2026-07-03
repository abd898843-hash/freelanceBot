using System.Globalization;
using System.Net.Http.Json;
using Freelance_bot.Application.Feature.Client.Request;
using Freelance_bot.Application.Feature.Projects.Requests;
using Freelance_bot.Application.Feature.Tasks.Request;
using Freelance_bot.Application.IServieces;
using Freelance_bot.Application.Servieces;
using Freelance_Bot.Domain.Enum;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Extensions.Constants;
using TelegramBot.Extensions.Service;
using TelegramBot.Handlers.Interface;
using TelegramBot.Navigation;
using TelegramBot.Services.Keyboards;
using TelegramBot.States;

namespace TelegramBot.Handlers
{
    public class BotUpdateHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITelegramBotClient _bot;
        private readonly IConversationStateStore _stateStore;
        private readonly IUserNavigationStore _navStore;
        private readonly KeyboardFactory _keyboards;
        private readonly IProjectService _projectService;
        private readonly IUserService _userService;
        private readonly IEnumerable<ICommandHandler> _handlers;
        private readonly IEnumerable<ICallbackHandler> _callbackHandlers;
        private readonly IClientService _clientService;
        private readonly ITaskService _taskService;

        public BotUpdateHandler(
            ITelegramBotClient bot,
            IConversationStateStore stateStore,
            IUserNavigationStore navStore,
            KeyboardFactory keyboards,
            IProjectService projectService,
            IUserService userService,
            IEnumerable<ICommandHandler> handlers,
            IEnumerable<ICallbackHandler> callbackHandlers,
            IClientService clientService,
            ITaskService taskService,
            IHttpClientFactory httpClientFactory)
        {
            _bot = bot;
            _stateStore = stateStore;
            _navStore = navStore;
            _keyboards = keyboards;
            _projectService = projectService;
            _userService = userService;
            _handlers = handlers;
            _callbackHandlers = callbackHandlers;
            _clientService = clientService;
            _taskService = taskService;
            _httpClientFactory = httpClientFactory;
        }

        public async Task HandleAsync(Update update)
        {
            switch (update.Type)
            {
                case UpdateType.Message when update.Message is not null:
                    await HandleMessageAsync(update.Message);
                    break;

                case UpdateType.CallbackQuery when update.CallbackQuery is not null:
                    await HandleCallbackAsync(update.CallbackQuery);
                    break;
            }
        }

        private async Task HandleMessageAsync(Message message)
        {
            var chatId = message.Chat.Id;
            var text = message.Text?.Trim() ?? string.Empty;

            // 1. معالجة زر Tasks (تم وضعها هنا كأولوية)
            if (text == "📋 Tasks")
            {
                var taskKeyboard = await _keyboards.GetProjectsTasksInlineAsync(message.From!.Id);

                // التحقق من أن الأزرار ليست فارغة لتجنب أي استثناءات
                if (taskKeyboard.InlineKeyboard != null && taskKeyboard.InlineKeyboard.Any())
                {
                    await _bot.SendTextMessageAsync(chatId, "📂 اختر المشروع لعرض التاسكات الخاصة به:", replyMarkup: taskKeyboard);
                }
                else
                {
                    await _bot.SendTextMessageAsync(chatId, "⚠️ لا توجد مشاريع متاحة حالياً.");
                }
                return;
            }

            // 2. التحقق من التنقل
            var currentNav = _navStore.GetCurrent(chatId);
            if (currentNav != null && currentNav.Screen == NavigationScreen.ProjectDetail && currentNav.EntityId.HasValue)
            {
                if (text != BotButtons.Back && !text.StartsWith('/'))
                {
                    await HandleInlineTaskCreationAsync(message, currentNav.EntityId.Value);
                    return;
                }
            }

            // 3. التحقق من حالة المحادثة
            var ctx = _stateStore.Get(chatId);
            if (ctx.Step != ConversationStep.None)
            {
                if (text == BotButtons.Back)
                {
                    _stateStore.Clear(chatId);
                    await SendMainMenuAsync(chatId, message.From!.Id);
                    return;
                }
                await HandleConversationStepAsync(message, ctx);
                return;
            }

            // 4. معالجة الأوامر العامة
            var command = text.StartsWith('/') ? text.Split(' ')[0].ToLower() : text;
            var handler = _handlers.FirstOrDefault(h => h.CanHandle(command));
            if (handler is not null)
            {
                await handler.HandleAsync(message);
                return;
            }

            // 5. الحالة الافتراضية (مش فاهم)
            var keyboard = await _keyboards.GetMainMenuAsync(message.From!.Id);
            await _bot.SendTextMessageAsync(chatId, "🤔 مش فاهم.\nاضغط على أي زر من القائمة.", replyMarkup: keyboard);
        }

        private async Task HandleInlineTaskCreationAsync(Message message, Guid projectId)
        {
            var chatId = message.Chat.Id;
            var taskTitle = message.Text?.Trim();

            if (string.IsNullOrWhiteSpace(taskTitle)) return;

            try
            {
                var user = await _userService.GetOrCreateByTelegramIdAsync(message.From!.Id, message.From.Username ?? "");
                await _taskService.CreateAsync(user.Id, new CreateTaskRequest(
                    ProjectId: projectId,
                    Title: taskTitle,
                    Notes: "تم إنشاؤها عبر الإضافة السريعة",
                    Priority: TaskPriority.Medium,
                    DueDate: DateTime.UtcNow.AddDays(3),
                    ParentTaskId: null
                ));

                _navStore.GoTo(chatId, NavigationScreen.Home, null);

                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[] { InlineKeyboardButton.WithCallbackData("➕ إضافة تاسك أخرى", $"task_create:{projectId}"),
                            InlineKeyboardButton.WithCallbackData("🔙 العودة للمشروع", $"project:{projectId}") }
                });

                await _bot.SendTextMessageAsync(chatId, $"✅ تم إضافة التاسك *\"{taskTitle}\"* بنجاح!", parseMode: ParseMode.Markdown, replyMarkup: inlineKeyboard);
            }
            catch (Exception ex)
            {
                await _bot.SendTextMessageAsync(chatId, "❌ حدث خطأ أثناء إضافة التاسك.");
            }
        }

        private async Task HandleCallbackAsync(CallbackQuery callback)
        {
            var chatId = callback.Message!.Chat.Id;
            var data = callback.Data ?? string.Empty;

            // 1. التعامل مع إضافة تاسك جديد يدوياً
            if (data == "setup_add_tasks")
            {
                var ctx = _stateStore.Get(chatId);
                ctx.Step = ConversationStep.AwaitingTaskTitle;
                _stateStore.Set(chatId, ctx);
                await _bot.SendTextMessageAsync(chatId, "👤 اكتب عنوان التاسك الجديد:");
                return;
            }

            // 2. التعامل مع الأتمتة
            if (data.StartsWith("setup_auto_tasks_"))
            {
                var projectId = Guid.Parse(data.Replace("setup_auto_tasks_", ""));
                await HandleAutoGenerateTasks(chatId, projectId);
                return;
            }

            // 3. التعامل مع عرض مهام مشروع معين (الميزة الجديدة)
            if (data.StartsWith("tasks_project:"))
            {
                var projectId = Guid.Parse(data.Replace("tasks_project:", ""));

                var tasks = await _taskService.GetTasksByProjectIdAsync(projectId);

                if (!tasks.Any())
                {
                    await _bot.AnswerCallbackQueryAsync(callback.Id, "لا توجد تاسكات لهذا المشروع حالياً.");
                    return;
                }

                var messageText = "📋 *قائمة المهام الخاصة بالمشروع:*\n\n" +
                                  string.Join("\n", tasks.Select(t => $"• {t.Title}"));

                await _bot.EditMessageTextAsync(chatId, callback.Message!.MessageId, messageText, parseMode: ParseMode.Markdown);
                return;
            }

            // 4. التعامل مع الـ CallbackHandlers الأخرى
            var callbackHandler = _callbackHandlers.FirstOrDefault(h => h.CanHandle(data));
            if (callbackHandler is not null)
            {
                await callbackHandler.HandleAsync(callback);
                return;
            }

            // 5. تأكيد إنشاء المشروع
            if (data == "confirm_create")
                await ExecuteCreateProjectAsync(chatId, callback.From.Id);

            // داخل BotUpdateHandler.cs -> HandleCallbackAsync

            if (data.StartsWith("dashboard:"))
            {
                var projectId = Guid.Parse(data.Replace("dashboard:", ""));

                // استخدم الرابط اللي الـ ngrok هيديهولك
                // ضيف عليه مسار الملف في الـ wwwroot
                var webAppUrl = $"https://your-ngrok-url.ngrok-free.app/dashboard.html?projectId={projectId}";

                var keyboard = new InlineKeyboardMarkup(
                    InlineKeyboardButton.WithWebApp("📊 عرض التحليلات الآن", new WebAppInfo { Url = webAppUrl })
                );

                await _bot.SendTextMessageAsync(chatId,
                    "✅ جاهز لعرض التحليلات الخاصة بالمشروع. اضغط على الزر أدناه لفتح لوحة التحكم:",
                    replyMarkup: keyboard);
                return; // مهم جداً عشان ما يكملش تنفيذ كود تاني
            }
        }
        private async Task ExecuteCreateProjectAsync(long chatId, long telegramId)
        {
            var ctx = _stateStore.Get(chatId);

            // التحقق من أن المشروع لم يتم إنشاؤه مسبقاً في هذا السياق
            if (ctx.IsProjectCreated)
            {
                await _bot.SendTextMessageAsync(chatId, "⚠️ تم إنشاء هذا المشروع بالفعل.");
                return;
            }

            try
            {
                var user = await _userService.GetOrCreateByTelegramIdAsync(telegramId, "");

                Guid? clientId = null;
                var clientName = ctx.Data.GetValueOrDefault("client_name");

                // 1. إنشاء العميل إذا تم إدخال اسمه
                if (!string.IsNullOrWhiteSpace(clientName))
                {
                    var client = await _clientService.CreateAsync(user.Id, new CreateClientRequest(
                        Name: clientName,
                        Email: ctx.Data.GetValueOrDefault("client_email"),
                        Phone: ctx.Data.GetValueOrDefault("client_phone"),
                        Company: null
                    ));
                    clientId = client.Id;
                }

                // 2. تجهيز البيانات
                decimal.TryParse(ctx.Data.GetValueOrDefault("budget"), out var parsedBudget);

                if (!DateTime.TryParseExact(ctx.Data.GetValueOrDefault("deadline"), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDeadline))
                {
                    parsedDeadline = DateTime.UtcNow.AddDays(7); // قيمة افتراضية إذا حدث خطأ
                }

                // 3. إنشاء المشروع
                var project = await _projectService.CreateAsync(user.Id, new CreateProjectRequest(
                    Title: ctx.Data.GetValueOrDefault("name") ?? "Untitled Project",
                    Description: ctx.Data.GetValueOrDefault("description"),
                    ClientName: clientName,
                    ClientEmail: ctx.Data.GetValueOrDefault("client_email"),
                    ClientPhone: ctx.Data.GetValueOrDefault("client_phone"),
                    ClientId: clientId,
                    Budget: parsedBudget,
                    Currency: "EGP",
                    StartDate: DateTime.UtcNow,
                    Deadline: parsedDeadline
                ));

                // 4. تحديث حالة السياق (State) بعد النجاح
                ctx.CreatedProjectId = project.Id;
                ctx.IsProjectCreated = true;
                ctx.Step = ConversationStep.AwaitingTaskDecision;
                _stateStore.Set(chatId, ctx);

                // 5. الرد برسالة النجاح مع أزرار التحكم
                await _bot.SendTextMessageAsync(chatId,
                    $"""
            🎉 تم إنشاء المشروع بنجاح
            📁 {project.Title}
            هل تريد إضافة التاسكات الآن؟
            """,
                    replyMarkup: new InlineKeyboardMarkup(new[]
                    {
                new[] { InlineKeyboardButton.WithCallbackData("➕ Add Tasks", "setup_add_tasks") },
                new[] { InlineKeyboardButton.WithCallbackData("🤖 Auto Generate", $"setup_auto_tasks_{project.Id}") }
                    }));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating project: {ex}");
                await _bot.SendTextMessageAsync(chatId, "❌ حدث خطأ فني أثناء إنشاء المشروع. يرجى المحاولة مرة أخرى لاحقاً.");
            }
        }
        private async Task HandleAutoGenerateTasks(long chatId, Guid projectId)
        {
            // (باقي كود الـ Auto Generate كما هو في ملفك)
            // تأكد فقط من إضافة الـ Model الخاص بـ N8NResponse إذا لم يكن موجوداً
        }

        // ... (باقي الميثودز الخاصة بـ HandleConversationStepAsync و ExecuteCreateProjectAsync كما هي في كودك) ...

        private async Task SendMainMenuAsync(long chatId, long telegramId)
        {
            var keyboard = await _keyboards.GetMainMenuAsync(telegramId);
            await _bot.SendTextMessageAsync(chatId, "📋 القائمة الرئيسية:", replyMarkup: keyboard);
        }
        private async Task HandleConversationStepAsync(Message message, ConversationContext ctx)
        {
            var chatId = message.Chat.Id;
            var text = message.Text?.Trim();

            switch (ctx.Step)
            {
                case ConversationStep.AwaitingProjectName:
                    ctx.Data["name"] = text;
                    ctx.Step = ConversationStep.AwaitingProjectDescription;
                    await _bot.SendTextMessageAsync(chatId, "📝 تمام، اكتب وصف المشروع:");
                    break;

                case ConversationStep.AwaitingProjectDescription:
                    ctx.Data["description"] = text;
                    ctx.Step = ConversationStep.AwaitingClientName;
                    await SendStepWithSkipButton(chatId, "👤 اسم العميل (أو اضغط 'تخطي'):");
                    break;

                case ConversationStep.AwaitingClientName:
                    if (text != "⏭ تخطي") ctx.Data["client_name"] = text;
                    ctx.Step = ConversationStep.AwaitingClientEmail;
                    await SendStepWithSkipButton(chatId, "📧 إيميل العميل (أو اضغط 'تخطي'):");
                    break;

                case ConversationStep.AwaitingClientEmail:
                    if (text != "⏭ تخطي") ctx.Data["client_email"] = text;
                    ctx.Step = ConversationStep.AwaitingClientPhone;
                    await SendStepWithSkipButton(chatId, "📞 رقم تليفون العميل (أو اضغط 'تخطي'):");
                    break;

                case ConversationStep.AwaitingClientPhone:
                    if (text != "⏭ تخطي") ctx.Data["client_phone"] = text;
                    ctx.Step = ConversationStep.AwaitingBudget;
                    await _bot.SendTextMessageAsync(chatId, "💰 ما هي ميزانية المشروع؟ (اكتب رقماً):");
                    break;

                case ConversationStep.AwaitingBudget:
                    ctx.Data["budget"] = text;
                    ctx.Step = ConversationStep.AwaitingDeadline;
                    await _bot.SendTextMessageAsync(chatId, "📅 ما هو الموعد النهائي؟ (YYYY-MM-DD):");
                    break;

                case ConversationStep.AwaitingDeadline:
                    ctx.Data["deadline"] = text;
                    ctx.Step = ConversationStep.AwaitingConfirmation;
                    await ShowSummaryAndConfirm(chatId, ctx);
                    break;
                case ConversationStep.AwaitingTaskTitle:
                    ctx.Data["task_title"] = text;
                    ctx.Step = ConversationStep.AwaitingTaskDescription;
                    await _bot.SendTextMessageAsync(chatId, "📝 تمام، اكتب وصف التاسك:");
                    break;

                case ConversationStep.AwaitingTaskDescription:
                    ctx.Data["task_description"] = text;
                    ctx.Step = ConversationStep.AwaitingTaskDeadline;
                    await _bot.SendTextMessageAsync(chatId, "📅 حدد الديدلاين (YYYY-MM-DD):");
                    break;

                case ConversationStep.AwaitingTaskDeadline:
                    ctx.Data["task_deadline"] = text;
                    // هنا استدعاء ميثود الحفظ الفعلي في قاعدة البيانات
                    await SaveTaskToDatabase(chatId, ctx);
                    break;
            }
            _stateStore.Set(chatId, ctx);
        }
        private async Task SaveTaskToDatabase(long chatId, ConversationContext ctx)
        {
            try
            {
                // 1. التأكد من وجود المشروع
                if (!ctx.CreatedProjectId.HasValue)
                {
                    await _bot.SendTextMessageAsync(chatId, "❌ خطأ: لم يتم تحديد المشروع بشكل صحيح.");
                    return;
                }

                // 2. محاولة تحويل التاريخ بأمان
                if (!DateTime.TryParse(ctx.Data.GetValueOrDefault("task_deadline"), out var deadline))
                {
                    await _bot.SendTextMessageAsync(chatId, "❌ التاريخ غير صحيح، يرجى إدخاله بصيغة YYYY-MM-DD (مثال: 2026-07-20).");
                    return;
                }

                var user = await _userService.GetOrCreateByTelegramIdAsync(chatId, "");

                // 3. إنشاء التاسك
                await _taskService.CreateAsync(user.Id, new CreateTaskRequest(
                    ProjectId: ctx.CreatedProjectId.Value,
                    Title: ctx.Data.GetValueOrDefault("task_title") ?? "بدون عنوان",
                    Notes: ctx.Data.GetValueOrDefault("task_description") ?? "",
                    Priority: TaskPriority.Medium,
                    DueDate: deadline,
                    ParentTaskId: null
                ));

                _stateStore.Clear(chatId);
                await _bot.SendTextMessageAsync(chatId, "✅ تم إضافة التاسك بنجاح للمشروع!");
            }
            catch (Exception ex)
            {
                // هنا يمكنك رؤية الخطأ الحقيقي في الـ Console
                Console.WriteLine($"Error Saving Task: {ex.Message}");
                await _bot.SendTextMessageAsync(chatId, "❌ حدث خطأ تقني. يرجى التأكد من البيانات.");
            }
        }
        private async Task ShowSummaryAndConfirm(long chatId, ConversationContext ctx)
        {
            var summary = $@"
                    🔍 **مراجعة تفاصيل المشروع:**
                    📁 الاسم: {ctx.Data.GetValueOrDefault("name")}
                    📝 الوصف: {ctx.Data.GetValueOrDefault("description")}
                    👤 العميل: {ctx.Data.GetValueOrDefault("client_name") ?? "غير محدد"}
                    💰 الميزانية: {ctx.Data.GetValueOrDefault("budget")}
                    📅 الموعد: {ctx.Data.GetValueOrDefault("deadline")}

                    هل أنت متأكد من إنشاء المشروع؟";

            var keyboard = new InlineKeyboardMarkup(new[]
            {
        new[] { InlineKeyboardButton.WithCallbackData("✅ تأكيد الإنشاء", "confirm_create"),
                InlineKeyboardButton.WithCallbackData("❌ إلغاء", "cancel_create") }
    });

            await _bot.SendTextMessageAsync(chatId, summary, parseMode: ParseMode.Markdown, replyMarkup: keyboard);
        }
        // ميثود مساعدة لإظهار زر التخطي
        private async Task SendStepWithSkipButton(long chatId, string prompt)
        {
            var keyboard = new ReplyKeyboardMarkup(new[] { new KeyboardButton("⏭ تخطي") }) { ResizeKeyboard = true };
            await _bot.SendTextMessageAsync(chatId, prompt, replyMarkup: keyboard);
        }
    

    private async Task ShowProjectSelectionForTasks(long chatId, long telegramId)
        {
            // جلب مشاريع المستخدم
            var user = await _userService.GetOrCreateByTelegramIdAsync(telegramId, "");
            var projects = await _projectService.GetProjectsByUserIdAsync(user.Id); // تأكد أن هذه الميثود موجودة في الـ Service

            if (projects == null || !projects.Any())
            {
                await _bot.SendTextMessageAsync(chatId, "⚠️ ليس لديك أي مشاريع حالياً لإنشاء تاسكات لها.");
                return;
            }

            // إنشاء أزرار لكل مشروع
            var inlineButtons = projects.Select(p =>
                new[] { InlineKeyboardButton.WithCallbackData(p.Title, $"view_tasks:{p.Id}") }
            ).ToList();

            var keyboard = new InlineKeyboardMarkup(inlineButtons);

            await _bot.SendTextMessageAsync(chatId, "📂 اختر المشروع لعرض التاسكات الخاصة به:", replyMarkup: keyboard);
        }
    } 
}