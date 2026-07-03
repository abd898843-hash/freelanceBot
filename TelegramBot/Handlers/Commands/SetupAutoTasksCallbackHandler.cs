//using Freelance_bot.Application.Feature.Automation;
//using Freelance_bot.Application.IServieces;
//using Freelance_Bot.Domain.Entity;
//using Freelance_Bot.Infrastruction.DB;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Net.Http;
//using System.Net.Http.Json;
//using System.Threading.Tasks;
//using Telegram.Bot;
//using Telegram.Bot.Types;
//using Telegram.Bot.Types.Enums;
//using Telegram.Bot.Types.ReplyMarkups;
//using TelegramBot.Handlers.Interface;

//namespace TelegramBot.Handlers.Commands
//{
//    public class SetupAutoTasksCallbackHandler : ICallbackHandler
//    {
//        private readonly IEventService _eventService;
//        private readonly ITelegramBotClient _botClient;
//        private readonly FreelancerDbContext _context;
//        private readonly HttpClient _httpClient; // أضفنا الـ HttpClient هنا للاتصال الفوري

//        public SetupAutoTasksCallbackHandler(
//            IEventService eventService,
//            ITelegramBotClient botClient,
//            FreelancerDbContext context,
//            HttpClient httpClient) // حقنه هنا
//        {
//            _eventService = eventService;
//            _botClient = botClient;
//            _context = context;
//            _httpClient = httpClient;
//        }

//        public bool CanHandle(string data)
//        {
//            // تغيير الشرط ليطابق الزرار المستخدم في البوت
//            return data == "setup_auto_tasks";
//        }

//        //public async Task HandleAsync(CallbackQuery callback)
//        //{
//        //    if (callback == null) return;

//        //    try
//        //    {
//        //        long telegramChatId = callback.Message?.Chat.Id ?? callback.From.Id;

//        //        // جلب المستخدم لضمان الـ Foreign Key
//        //        var dbUser = await _context.users.FirstOrDefaultAsync(u => u.TelegramChatId == telegramChatId);

//        //        if (dbUser == null)
//        //        {
//        //            await _botClient.AnswerCallbackQueryAsync(callback.Id, "⚠️ لم يتم العثور على حسابك المسجل.");
//        //            return;
//        //        }

//        //        // استخراج الـ Project Guid
//        //        Guid projectId = Guid.Empty;
//        //        if (!string.IsNullOrEmpty(callback.Data))
//        //        {
//        //            string[] parts = callback.Data.Split('_');
//        //            if (parts.Length >= 4 && Guid.TryParse(parts[3], out Guid id)) projectId = id;
//        //            else if (parts.Length >= 3 && Guid.TryParse(parts[2], out Guid idAlternative)) projectId = idAlternative;
//        //        }

//        //        // 1. نشر الحدث محلياً في الداتا بايز (كودك القديم)
//        //        var payload = new GenerateTasksPayload
//        //        {
//        //            ProjectId = projectId,
//        //            ProjectTitle = "طلب توليد آلي للمشروع",
//        //            Description = "جاري قراءة التفاصيل وتوليد المهام عبر n8n والذكاء الاصطناعي.",
//        //            Deadline = DateTime.UtcNow.AddDays(7),
//        //            Budget = 0
//        //        };

//        //        await _eventService.PublishAsync(
//        //            userId: dbUser.Id,
//        //            entityType: "Project",
//        //            entityId: projectId,
//        //            eventName: "GenerateTasksRequested",
//        //            payloadData: payload
//        //        );

//        //        await _botClient.AnswerCallbackQueryAsync(callback.Id, "🚀 جاري تشغيل ذكاء n8n التلقائي...");

//        //        // ======================================================================
//        //        // 🔥 هنا بنحط رابط التيست اللي اديتهولي عشان نضرب الـ Webhook بتاع n8n
//        //        // ======================================================================
//        //        var n8nTestUrl = "http://localhost:5678/webhook-test/create-project-tasks";

//        //        using (var httpClient = new HttpClient())
//        //        {
//        //            var n8nPayload = new
//        //            {
//        //                userId = dbUser.Id,
//        //                projectId = projectId,
//        //                chatId = telegramChatId
//        //            };

//        //            // إرسال الـ POST Request إلى n8n
//        //            // إرسال الـ POST Request إلى n8n
//        //            var response = await httpClient.PostAsJsonAsync(n8nTestUrl, n8nPayload);

//        //            if (response.IsSuccessStatusCode)
//        //            {
//        //                // 1. اقرأ التاسكات اللي راجعة من n8n كـ List
//        //                var tasks = await response.Content.ReadFromJsonAsync<List<TaskItemDto>>(); // تأكد من اسم الـ DTO بتاعك

//        //                // 2. ابني رسالة شيك بالتاسكات
//        //                string message = "✅ *تم توليد المهام بنجاح:*\n\n";
//        //                foreach (var task in tasks)
//        //                {
//        //                    message += $"🔹 *{task.Title}*\n   {task.Notes}\n\n";
//        //                }

//        //                // 3. ضيف أزرار (الرجوع أو إضافة المهام)
//        //                var inlineKeyboard = new InlineKeyboardMarkup(new[]
//        //                {
//        //                        new [] { InlineKeyboardButton.WithCallbackData("➕ إضافة المهام للداتابيز", $"save_tasks_{projectId}") },
//        //                        new [] { InlineKeyboardButton.WithCallbackData("⬅️ رجوع للمشروع", $"view_project_{projectId}") }
//        //                    });

//        //                await _botClient.SendTextMessageAsync(
//        //                    chatId: callback.Message.Chat.Id,
//        //                    text: message,
//        //                    parseMode: ParseMode.Markdown,
//        //                    replyMarkup: inlineKeyboard
//        //                );
//        //            }
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        await _botClient.AnswerCallbackQueryAsync(callback.Id, "⚠️ حدث خطأ داخلي.");
//        //        Console.WriteLine($"Error: {ex.Message}");
//        //    }


//        //public async Task HandleAsync(CallbackQuery callback)
//        //{
//        //    if (callback == null) return;

//        //    // 1. الرد الفوري (يجب أن يكون أول شيء بدون أي عمليات ثقيلة قبله)
//        //    try
//        //    {
//        //        await _botClient.AnswerCallbackQueryAsync(callback.Id, "🚀 جاري المعالجة...");
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        Console.WriteLine($"Error answering callback: {ex.Message}");
//        //    }

//        //    // 2. العمليات الثقيلة (قاعدة البيانات و n8n)
//        //    try
//        //    {
//        //        long telegramChatId = callback.Message?.Chat.Id ?? callback.From.Id;
//        //        var dbUser = await _context.users.FirstOrDefaultAsync(u => u.TelegramChatId == telegramChatId);

//        //        if (dbUser == null) return;

//        //        // استخراج الـ projectId
//        //        Guid projectId = Guid.Empty;
//        //        if (!string.IsNullOrEmpty(callback.Data))
//        //        {
//        //            string[] parts = callback.Data.Split('_');
//        //            if (parts.Length >= 4 && Guid.TryParse(parts[3], out Guid id)) projectId = id;
//        //            else if (parts.Length >= 3 && Guid.TryParse(parts[2], out Guid idAlternative)) projectId = idAlternative;
//        //        }

//        //        // استخدام رابط الـ Production (بدون كلمة -test)
//        //        // غيّر الرابط لـ webhook-test
//        //        var n8nUrl = "https://iodize-tank-sector.ngrok-free.dev/webhook-test/create-project-tasks";

//        //        using (var httpClient = new HttpClient())
//        //        {
//        //            var n8nPayload = new { userId = dbUser.Id, projectId = projectId, chatId = telegramChatId };
//        //            var response = await httpClient.PostAsJsonAsync(n8nUrl, n8nPayload);

//        //            if (response.IsSuccessStatusCode)
//        //            {
//        //                var tasks = await response.Content.ReadFromJsonAsync<List<TaskItemDto>>();
//        //                await SendTasksToTelegram(callback.Message.Chat.Id, tasks, projectId);
//        //            }
//        //            else
//        //            {
//        //                await _botClient.SendTextMessageAsync(callback.Message.Chat.Id, "⚠️ فشل الاتصال بـ n8n.");
//        //            }
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        Console.WriteLine($"Error in background process: {ex.Message}");
//        //        await _botClient.SendTextMessageAsync(callback.Message.Chat.Id, "❌ حدث خطأ داخلي أثناء معالجة المهام.");
//        //    }
//        //}

//        public async Task HandleAsync(CallbackQuery callback)
//        {
//            if (callback == null) return;

//            // 1. رد فوري للتيليجرام عشان ميبانش إن البوت علّق
//            await _botClient.AnswerCallbackQueryAsync(callback.Id, "🚀 جاري المعالجة...");

//            try
//            {
//                long chatId = callback.Message!.Chat.Id;
//                var dbUser = await _context.users.FirstOrDefaultAsync(u => u.TelegramChatId == chatId);
//                if (dbUser == null) return;

//                // استخراج الـ projectId من الـ data
//                Guid projectId = Guid.Empty;
//                var parts = callback.Data!.Split('_');
//                Guid.TryParse(parts.Last(), out projectId);

//                // الرابط اللي بيبعت للـ n8n
//                var n8nUrl = "https://iodize-tank-sector.ngrok-free.dev/webhook-test/create-project-tasks";

//                // استخدام الـ _httpClient اللي عملنا له حقن في الـ Constructor
//                var n8nPayload = new { userId = dbUser.Id, projectId = projectId, chatId = chatId };
//                var response = await _httpClient.PostAsJsonAsync(n8nUrl, n8nPayload);

//                if (response.IsSuccessStatusCode)
//                {
//                    var tasks = await response.Content.ReadFromJsonAsync<List<TaskItemDto>>();
//                    await SendTasksToTelegram(chatId, tasks, projectId);
//                }
//                else
//                {
//                    await _botClient.SendTextMessageAsync(chatId, "⚠️ فشل الاتصال بـ n8n. تأكد إنه شغال.");
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error: {ex.Message}");
//                await _botClient.SendTextMessageAsync(callback.Message.Chat.Id, "❌ حدث خطأ.");
//            }
//        }

//        private async Task SendTasksToTelegram(long chatId, List<TaskItemDto> tasks, Guid projectId)
//        {
//            if (tasks == null || tasks.Count == 0)
//            {
//                await _botClient.SendTextMessageAsync(chatId, "⚠️ لم يتم العثور على مهام لتوليدها.");
//                return;
//            }

//            string message = "✅ *تم توليد المهام بنجاح:*\n\n";
//            foreach (var task in tasks)
//            {
//                message += $"🔹 *{task.Title}*\n   {task.Notes}\n\n";
//            }

//            var inlineKeyboard = new InlineKeyboardMarkup(new[]
//                            {
//                        new [] { InlineKeyboardButton.WithCallbackData("➕ إضافة المهام للداتابيز", $"save_tasks_{projectId}") },
//                        new [] { InlineKeyboardButton.WithCallbackData("⬅️ رجوع للمشروع", $"view_project_{projectId}") }
//                    });

//            await _botClient.SendTextMessageAsync(
//                chatId: chatId,
//                text: message,
//                parseMode: ParseMode.Markdown,
//                replyMarkup: inlineKeyboard
//            );
//        }
//    }
//}

using Freelance_bot.Application.Feature.Automation;
using Freelance_bot.Application.IServieces;
using Freelance_Bot.Infrastruction.DB;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Handlers.Interface;
using TelegramBot.States;

namespace TelegramBot.Handlers.Commands
{
    public class SetupAutoTasksCallbackHandler : ICallbackHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly FreelancerDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConversationStateStore _stateStore;

        public SetupAutoTasksCallbackHandler(
            ITelegramBotClient botClient,
            FreelancerDbContext context,
            HttpClient httpClient,
            IConversationStateStore stateStore)
        {
            _botClient = botClient;
            _context = context;
            _httpClient = httpClient;
            _stateStore = stateStore;
        }

        public bool CanHandle(string data) => data == "setup_auto_tasks";

        public async Task HandleAsync(CallbackQuery callback)
        {
            if (callback == null) return;
            await _botClient.AnswerCallbackQueryAsync(callback.Id, "🤖 جاري التواصل مع الذكاء الاصطناعي...");

            try
            {
                long chatId = callback.Message!.Chat.Id;
                var ctx = _stateStore.Get(chatId);
                Guid projectId = ctx.CreatedProjectId ?? Guid.Empty;

                if (projectId == Guid.Empty)
                {
                    await _botClient.SendTextMessageAsync(chatId, "⚠️ لم يتم العثور على مشروع نشط.");
                    return;
                }

                var dbUser = await _context.users.FirstOrDefaultAsync(u => u.TelegramChatId == chatId);
                var n8nUrl = "https://iodize-tank-sector.ngrok-free.dev/webhook-test/create-project-tasks";

                var n8nPayload = new { userId = dbUser?.Id, projectId = projectId, chatId = chatId };
                var response = await _httpClient.PostAsJsonAsync(n8nUrl, n8nPayload);

                // داخل HandleAsync
                if (response.IsSuccessStatusCode)
                {
                    // استقبل مصفوفة من AutomationTaskDto
                    var tasks = await response.Content.ReadFromJsonAsync<List<AutomationTaskDto>>();

                    // تخزينها في الـ State (تأكد أن الـ State Store يدعم List<AutomationTaskDto>)
                
                    ctx.TemporaryAutomationTasks = tasks;
                    _stateStore.Set(chatId, ctx);

                    await SendTasksToTelegram(chatId, tasks, projectId);
                }
                else
                {
                    await _botClient.SendTextMessageAsync(chatId, "⚠️ فشل الاتصال بـ n8n.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                await _botClient.SendTextMessageAsync(callback.Message.Chat.Id, "❌ حدث خطأ داخلي.");
            }
        }

        // غير النوع من TaskItemDto إلى AutomationTaskDto
        private async Task SendTasksToTelegram(long chatId, List<AutomationTaskDto> tasks, Guid projectId)
        {
            if (tasks == null || !tasks.Any())
            {
                await _botClient.SendTextMessageAsync(chatId, "⚠️ لم يتم العثور على مهام لتوليدها.");
                return;
            }

            string message = "✅ *تم توليد المهام بنجاح:*\n\n";
            foreach (var task in tasks)
            {
                message += $"🔹 *{task.Title}*\n{task.Notes}\n\n";
            }

            var kb = new InlineKeyboardMarkup(new[] {
        new[] { InlineKeyboardButton.WithCallbackData("➕ إضافة للداتابيز", $"save_tasks_{projectId}") },
        new[] { InlineKeyboardButton.WithCallbackData("⬅️ رجوع", $"project:{projectId}") }
    });

            await _botClient.SendTextMessageAsync(chatId, message, parseMode: ParseMode.Markdown, replyMarkup: kb);
        }
    }
}