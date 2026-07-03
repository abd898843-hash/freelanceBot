using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Handlers.Interface;

public class DashboardCallbackHandler : ICallbackHandler
{
    private readonly ITelegramBotClient _bot;

    public DashboardCallbackHandler(ITelegramBotClient bot)
    {
        _bot = bot;
    }

    public bool CanHandle(string data) => data.StartsWith("dashboard:");

    public async Task HandleAsync(CallbackQuery callback)
    {
        var projectId = callback.Data!.Split(':')[1];
        var url = $"https://iodize-tank-sector.ngrok-free.dev/dashboard.html?id={projectId}";

        var keyboard = new InlineKeyboardMarkup(InlineKeyboardButton.WithWebApp(
    "📊 Open Dashboard",
    new WebAppInfo { Url = url })); // التعديل هنا

        await _bot.SendTextMessageAsync(
            callback.Message!.Chat.Id,
            "📊 اضغط على الزر أدناه لفتح لوحة التحكم:",
            replyMarkup: keyboard);
    }
}