using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Constants;
using TelegramBot.Handlers.Interface;


namespace TelegramBot.Handlers.Commands
{
    public class ReportProjectCallbackHandler : ICallbackQueryHandler
    {
        private readonly ITelegramBotClient _bot;

        public ReportProjectCallbackHandler(ITelegramBotClient bot) => _bot = bot;

        public bool CanHandle(string data) => data.StartsWith(CallbackPrefixes.ReportProject);

        public async Task HandleAsync(CallbackQuery callback)
        {
            // إنهاء حالة الـ Loading في التليجرام
            await _bot.AnswerCallbackQueryAsync(callback.Id);

            var parts = callback.Data!.Split(':');
            var projectId = Guid.Parse(parts[1]);

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
        new[] { InlineKeyboardButton.WithCallbackData("📊 Daily Report", $"{CallbackPrefixes.ReportType}:{(int)ReportType.Daily}:{projectId}") },
        new[] { InlineKeyboardButton.WithCallbackData("📅 Weekly Report", $"{CallbackPrefixes.ReportType}:{(int)ReportType.Weekly}:{projectId}") },
        new[] { InlineKeyboardButton.WithCallbackData("📨 Follow-up Report", $"{CallbackPrefixes.ReportType}:{(int)ReportType.FollowUp}:{projectId}") },
        new[] { InlineKeyboardButton.WithCallbackData("⬅ Back", CallbackPrefixes.BackToReports) }
    });

            await _bot.EditMessageTextAsync(callback.Message!.Chat.Id, callback.Message.MessageId, "📄 اختر نوع التقرير:", replyMarkup: inlineKeyboard);
        }

    }
}