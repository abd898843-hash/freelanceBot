using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Constants;
using TelegramBot.Handlers.Interface;
using TelegramBot.Navigation;

public class ReportTypeCallbackHandler : ICallbackQueryHandler
{
    private readonly ITelegramBotClient _bot;
    private readonly IUserNavigationStore _navStore;

    public ReportTypeCallbackHandler(ITelegramBotClient bot, IUserNavigationStore navStore)
    {
        _bot = bot;
        _navStore = navStore;
    }

    public bool CanHandle(string data) => data.StartsWith(CallbackPrefixes.ReportType);

    public async Task HandleAsync(CallbackQuery callback)
    {
        await _bot.AnswerCallbackQueryAsync(callback.Id);

        // الصيغة: report_type:{typeIndex}:{projectId}
        var parts = callback.Data!.Split(':');
        var reportType = parts[1]; // 0, 1, 2
        var projectId = parts[2];

        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            new[] { InlineKeyboardButton.WithCallbackData("⚡ Generate Now", $"report_action:generate:{reportType}:{projectId}") },
            new[] { InlineKeyboardButton.WithCallbackData("⏰ Schedule", $"report_action:schedule:{reportType}:{projectId}") },
            new[] { InlineKeyboardButton.WithCallbackData("⬅ Back", CallbackPrefixes.BackToReports) }
        });

        await _bot.EditMessageTextAsync(
            callback.Message!.Chat.Id,
            callback.Message.MessageId,
            "📊 اختر الإجراء المطلوب:",
            replyMarkup: inlineKeyboard);
    }
}