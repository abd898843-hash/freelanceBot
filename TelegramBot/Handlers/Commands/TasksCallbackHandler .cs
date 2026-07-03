using Freelance_bot.Application.IServieces;
using Freelance_bot.Application.Servieces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Handlers.Interface;

namespace TelegramBot.Handlers.Callbacks;

     
public class TasksCallbackHandler : ICallbackHandler
{
    private readonly ITelegramBotClient _bot;
    private readonly ITaskService _taskService;
    private readonly IUserService _userService;

    public TasksCallbackHandler(
        ITelegramBotClient bot,
        ITaskService taskService,
        IUserService userService)
    {
        _bot = bot;
        _taskService = taskService;
        _userService = userService;
    }

    public bool CanHandle(string data)
        => data.StartsWith("tasks:");

    public async Task HandleAsync(CallbackQuery callback)
    {
       

     

        if (callback.Message is null)
            return;

        var projectId =
            Guid.Parse(callback.Data!.Split(':')[1]);

        // ⚠️ استخدم userId الحقيقي عندك
        var user =
      await _userService
      .GetOrCreateByTelegramIdAsync(
          callback.From.Id,
          callback.From.Username ?? "");

        var userId = user.Id;

        var tasks =
            (await _taskService.GetByProjectAsync(projectId, userId))
            .ToList();

        if (!tasks.Any())
        {
            await _bot.SendTextMessageAsync(
                callback.Message.Chat.Id,
                "📭 مفيش Tasks فى المشروع ده");
            return;
        }

        var buttons = tasks.Select(t =>
            new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    $"{GetStatusIcon(t.Status)} {t.Title}",
                    $"task_open:{t.Id}")
            }).ToArray();

        var inline = new InlineKeyboardMarkup(buttons);

        await _bot.SendTextMessageAsync(
            callback.Message.Chat.Id,
            "📋 Tasks:",
            replyMarkup: inline);
    }

    private string GetStatusIcon(object status)
        => status.ToString() switch
        {
            "Done" => "✅",
            "InProgress" => "⏳",
            "Todo" => "📝",
            "Cancelled" => "❌",
            _ => "📌"
        };
}

