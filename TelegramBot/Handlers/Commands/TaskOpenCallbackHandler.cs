using Freelance_bot.Application.IServieces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Handlers.Interface;

namespace TelegramBot.Handlers.Callbacks;

public class TaskOpenCallbackHandler : ICallbackHandler
{
    private readonly ITelegramBotClient _bot;
    private readonly ITaskService _taskService;
    private readonly IUserService _userService;

    public TaskOpenCallbackHandler(
        ITelegramBotClient bot,
        ITaskService taskService,
        IUserService userService)
    {
        _bot = bot;
        _taskService = taskService;
        _userService = userService;
    }

    public bool CanHandle(string data)
        => data.StartsWith("task_open:");

    public async Task HandleAsync(CallbackQuery callback)
    {
        if (callback.Message is null)
            return;

        var taskId = Guid.Parse(callback.Data!.Split(':')[1]);

        var user = await _userService.GetOrCreateByTelegramIdAsync(
                callback.From.Id,
                callback.From.Username ?? "");

        // التصحيح: جلب التاسك المحددة لمعرفة بياناتها وحالتها
        var task = await _taskService.GetByIdAsync(taskId, user.Id);

        // بناء قائمة الأزرار ديناميكياً
        var keyboardButtons = new List<InlineKeyboardButton[]>();

        if (task.Status != Freelance_Bot.Domain.Enum.TaskStatus.Done)
        {
            string moveButtonText = task.Status == Freelance_Bot.Domain.Enum.TaskStatus.Todo
                ? "⏳ بدء العمل (In Progress)"
                : "✅ نقل إلى المنتهية (Done)";

            keyboardButtons.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData(moveButtonText, $"task_move:{taskId}")
            });
        }
        else
        {
            keyboardButtons.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData("↩️ إعادة فتح (Todo)", $"task_move:{taskId}")
            });
        }

        // إضافة زر الحذف
        keyboardButtons.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData("🗑 Delete Task", $"task_delete:{taskId}")
        });

        var messageText = $"""
        📌 *تفاصيل التاسك:*
        ━━━━━━━━━━━━━━
        📝 *العنوان:* {task.Title}
        📊 *الحالة الحالية:* {GetStatusIcon(task.Status)} {task.Status}
        """;

        await _bot.SendTextMessageAsync(
            callback.Message.Chat.Id,
            messageText,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            replyMarkup: new InlineKeyboardMarkup(keyboardButtons));
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