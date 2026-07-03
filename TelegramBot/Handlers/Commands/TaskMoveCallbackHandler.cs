using Freelance_bot.Application.Feature.Tasks.Request;
using Freelance_bot.Application.IServieces;
using Freelance_Bot.Domain.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Handlers.Interface;

namespace TelegramBot.Handlers.Callbacks
{
    public class TaskMoveCallbackHandler : ICallbackHandler
    {
        private readonly ITelegramBotClient _bot;
        private readonly ITaskService _taskService;
        private readonly IUserService _userService;

        public TaskMoveCallbackHandler(
            ITelegramBotClient bot,
            ITaskService taskService,
            IUserService userService)
        {
            _bot = bot;
            _taskService = taskService;
            _userService = userService;
        }

        public bool CanHandle(string data)
            => data.StartsWith("task_move:");

        public async Task HandleAsync(CallbackQuery callback)
        {
            if (callback.Message is null)
                return;

            // 1. استخراج الـ ID وتحديث الحالة
            var taskId = Guid.Parse(callback.Data!.Split(':')[1]);
            var user = await _userService.GetOrCreateByTelegramIdAsync(
                callback.From.Id,
                callback.From.Username ?? "");

            var task = await _taskService.GetByIdAsync(taskId, user.Id);
            var newStatus = task.Status.Next();

            // تحديث الحالة في الداتا بيز
            await _taskService.UpdateAsync(
                taskId,
                user.Id,
                new UpdateTaskRequest(null, null, newStatus, null, null));

            // 2. بناء الأزرار الديناميكية للتفاعل مع الحالة الجديدة
            var keyboardButtons = new List<InlineKeyboardButton[]>();

            // زر التبديل للحالة التالية
            if (newStatus != Freelance_Bot.Domain.Enum.TaskStatus.Done)
            {
                string moveButtonText = newStatus == Freelance_Bot.Domain.Enum.TaskStatus.Todo
                    ? "⏳ بدء العمل"
                    : "✅ إنهاء";

                keyboardButtons.Add(new[] { InlineKeyboardButton.WithCallbackData(moveButtonText, $"task_move:{taskId}") });
            }
            else
            {
                keyboardButtons.Add(new[] { InlineKeyboardButton.WithCallbackData("↩️ إعادة فتح", $"task_move:{taskId}") });
            }

            // أزرار التحكم: الحذف + الرجوع للوحة الكانبان (مهم جداً لتحديث الصورة كما في image_bffec9.png)
            keyboardButtons.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData("🗑 حذف", $"task_delete:{taskId}"),
                InlineKeyboardButton.WithCallbackData("🔙 العودة للكانبان", $"kanban:{task.ProjectId}")
            });

            // 3. تجهيز نص التحديث
            var updatedText = $"""
            📌 *تحديث حالة التاسك:*
            ━━━━━━━━━━━━━━
            📝 *العنوان:* {task.Title}
            📊 *الحالة الجديدة:* {GetStatusIcon(newStatus)} {newStatus}
            """;

            // 4. تحديث الرسالة الحالية
            await _bot.EditMessageTextAsync(
                chatId: callback.Message.Chat.Id,
                messageId: callback.Message.MessageId,
                text: updatedText,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: new InlineKeyboardMarkup(keyboardButtons));

            // إشعار للمستخدم
            await _bot.AnswerCallbackQueryAsync(callback.Id, "تم تحديث الحالة بنجاح!");
        }

        private string GetStatusIcon(object status)
            => status.ToString() switch
            {
                "Done" => "✅",
                "InProgress" => "⏳",
                "Todo" => "📝",
                _ => "📌"
            };
    }
}