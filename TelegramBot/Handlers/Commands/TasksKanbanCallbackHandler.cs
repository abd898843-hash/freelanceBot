using System.Text;
using Freelance_bot.Application.Feature.Tasks.Response;
using Freelance_bot.Application.IServieces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Handlers.Interface;
using TaskStatus = Freelance_Bot.Domain.Enum.TaskStatus;

namespace TelegramBot.Handlers.Callbacks
{
    public class TasksKanbanCallbackHandler : ICallbackHandler
    {
        private readonly ITelegramBotClient _bot;
        private readonly ITaskService _taskService;
        private readonly IUserService _userService;

        public TasksKanbanCallbackHandler(
            ITelegramBotClient bot,
            ITaskService taskService,
            IUserService userService)
        {
            _bot = bot;
            _taskService = taskService;
            _userService = userService;
        }

        public bool CanHandle(string data)
            => data.StartsWith("kanban:");

        public async Task HandleAsync(CallbackQuery callback)
        {
            if (callback.Message is null)
                return;

            var projectId = Guid.Parse(callback.Data!.Split(':')[1]);
            var user = await _userService.GetOrCreateByTelegramIdAsync(
                   callback.From.Id,
                   callback.From.Username ?? "");

            var tasks = (await _taskService.GetByProjectAsync(projectId, user.Id)).ToList();

            var todo = tasks.Where(t => t.Status == TaskStatus.Todo).ToList();
            var inProgress = tasks.Where(t => t.Status == TaskStatus.InProgress).ToList();
            var done = tasks.Where(t => t.Status == TaskStatus.Done).ToList();

            var text = BuildKanbanText(todo, inProgress, done);

            await _bot.SendTextMessageAsync(
                callback.Message.Chat.Id,
                text,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
              replyMarkup: BuildKanbanKeyboard(tasks, projectId)
            );
        }

        private string BuildKanbanText(
            List<TaskResponse> todo,
            List<TaskResponse> inProgress,
            List<TaskResponse> done)
        {
            var sb = new StringBuilder();

            sb.AppendLine("📊 *KANBAN BOARD*");
            sb.AppendLine("━━━━━━━━━━━━━━━━━━");
            sb.AppendLine();

            // Section: TODO
            sb.AppendLine($"📝 *TODO ({todo.Count})*");
            if (todo.Any())
                foreach (var task in todo) sb.AppendLine($"• {task.Title}");
            else
                sb.AppendLine("لا يوجد");

            sb.AppendLine();

            // Section: IN PROGRESS
            sb.AppendLine($"⏳ *IN PROGRESS ({inProgress.Count})*");
            if (inProgress.Any())
                foreach (var task in inProgress) sb.AppendLine($"• {task.Title}");
            else
                sb.AppendLine("لا يوجد");

            sb.AppendLine();

            // Section: DONE
            sb.AppendLine($"✅ *DONE ({done.Count})*");
            if (done.Any())
                foreach (var task in done) sb.AppendLine($"• {task.Title}");
            else
                sb.AppendLine("لا يوجد");

            return sb.ToString();
        }

        private InlineKeyboardMarkup BuildKanbanKeyboard(List<TaskResponse> tasks, Guid projectId)
        {
            var buttonsList = new List<InlineKeyboardButton[]>();

            foreach (var task in tasks)
            {
                // الزر يعرض اسم التاسك وأيقونة حالتها
                // عند الضغط، سيقوم الـ Handler الخاص بـ task_move بتغيير حالتها
                string statusIcon = GetStatusIcon(task.Status);
                buttonsList.Add(new[]
                {
            InlineKeyboardButton.WithCallbackData($"{statusIcon} {task.Title}", $"task_move:{task.Id}")
        });
            }

            // زر الإضافة في الأسفل
            buttonsList.Add(new[]
            {
        InlineKeyboardButton.WithCallbackData("➕ إضافة تاسك للمشروع", $"task_create:{projectId}")
    });

            return new InlineKeyboardMarkup(buttonsList);
        }
        private string GetStatusIcon(Freelance_Bot.Domain.Enum.TaskStatus status)
        {
            return status switch
            {
                Freelance_Bot.Domain.Enum.TaskStatus.Done => "✅",
                Freelance_Bot.Domain.Enum.TaskStatus.InProgress => "⏳",
                Freelance_Bot.Domain.Enum.TaskStatus.Todo => "📝",
                _ => "📌"
            };
        }
    }
}