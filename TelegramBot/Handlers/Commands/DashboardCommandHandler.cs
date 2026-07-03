using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Extensions.Constants;
using TelegramBot.Handlers.Interface;

namespace TelegramBot.Handlers.Commands;

public class DashboardCommandHandler : ICommandHandler
{
    private readonly ITelegramBotClient _bot;

    public DashboardCommandHandler(
        ITelegramBotClient bot)
    {
        _bot = bot;
    }

    public bool CanHandle(string input)
        => input is "/dashboard" or BotButtons.Dashboard;

    public async Task HandleAsync(Message message)
    {
        var keyboard = new InlineKeyboardMarkup(
    InlineKeyboardButton.WithWebApp(
        "🚀 عرض التحليلات الخاصة بك",
        new WebAppInfo
        {
            Url = "https://iodize-tank-sector.ngrok-free.dev/dashboard.html"
        }
    )
);
        await _bot.SendTextMessageAsync(

            chatId: message.Chat.Id,

            text:
@"📊 Dashboard

يمكنك من خلال لوحة التحكم متابعة:

• عدد المشاريع

• عدد التاسكات

• الدخل

• نسب الإنجاز

• المشاريع الحالية

• المواعيد النهائية

اضغط الزر بالأسفل لفتح لوحة التحكم.",

            replyMarkup: keyboard);
    }
}