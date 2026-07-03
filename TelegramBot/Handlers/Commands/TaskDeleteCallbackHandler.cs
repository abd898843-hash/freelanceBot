using Freelance_bot.Application.IServieces;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Handlers.Interface;

namespace TelegramBot.Handlers.Callbacks
{
    public class TaskDeleteCallbackHandler : ICallbackHandler
    {
        private readonly ITelegramBotClient _bot;
        private readonly ITaskService _taskService;
        private readonly IUserService _userService;

        public TaskDeleteCallbackHandler(ITelegramBotClient bot, ITaskService taskService, IUserService userService)
        {
            _bot = bot;
            _taskService = taskService;
            _userService = userService;
        }

        public bool CanHandle(string data) => data.StartsWith("task_delete:");

        public async Task HandleAsync(CallbackQuery callback)
        {
            if (callback.Message is null) return;

            var taskId = Guid.Parse(callback.Data!.Split(':')[1]);
            var user = await _userService.GetOrCreateByTelegramIdAsync(
                callback.From.Id,
                callback.From.Username ?? "");

            var userId = user.Id;

            // تنفيذ الحذف في الداتا بيز
            await _taskService.DeleteAsync(taskId, userId);

            // تعديل الرسالة الحالية لإعلام المستخدم بالحذف ومسح الأزرار القديمة
            await _bot.EditMessageTextAsync(
                chatId: callback.Message.Chat.Id,
                messageId: callback.Message.MessageId,
                text: "🗑 تم حذف التاسك بنجاح."
            );
        }
    }
}