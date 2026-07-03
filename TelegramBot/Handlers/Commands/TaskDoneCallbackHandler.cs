//using Freelance_bot.Application.Feature.Tasks.Request;
//using Freelance_bot.Application.IServieces;
//using Freelance_bot.Application.Servieces;
//using Telegram.Bot;
//using Telegram.Bot.Types;
//using TelegramBot.Handlers.Interface;

//namespace TelegramBot.Handlers.Callbacks
//{
//    public class TaskDoneCallbackHandler : ICallbackHandler
//    {
//        private readonly ITelegramBotClient _bot;
//        private readonly ITaskService _taskService;
//        private readonly IUserService _userService;

//        public TaskDoneCallbackHandler(
//            ITelegramBotClient bot,
//            ITaskService taskService,
//            IUserService userService)
//        {
//            _bot = bot;
//            _taskService = taskService;
//            _userService = userService;
//        }

//        public bool CanHandle(string data)
//            => data.StartsWith("task_done:");

//        public async Task HandleAsync(CallbackQuery callback)
//        {
//            if (callback.Message is null)
//                return;

//            var taskId = Guid.Parse(callback.Data!.Split(':')[1]);

//            var user =
//      await _userService
//      .GetOrCreateByTelegramIdAsync(
//          callback.From.Id,
//          callback.From.Username ?? "");

//            var userId = user.Id;

//            // ====================================================
//            // Move task forward (KANBAN STYLE)
//            // ====================================================

//            await _taskService.UpdateAsync(
//    taskId,
//    userId,
//    new UpdateTaskRequest(
//        Title: null,
//        Notes: null,
//        Status: null,
//        Priority: null,
//        DueDate: null,
//        value: null
//    )
//            );

//            await _bot.SendTextMessageAsync(
//                callback.Message.Chat.Id,
//                "✅ تم تحديث حالة التاسك (Kanban Flow)");
//        }
//    }
//}