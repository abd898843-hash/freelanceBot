//using Freelance_bot.Application.IServieces;
//using Telegram.Bot;
//using Telegram.Bot.Types;
//using Telegram.Bot.Types.ReplyMarkups;
//using TelegramBot.Handlers.Interface;

//namespace TelegramBot.Handlers.Commands
//{
//    public class ReportActionCallbackHandler : ICallbackQueryHandler
//    {
//        private readonly ITelegramBotClient _bot;
//        private readonly IReportService _reportService;

//        public ReportActionCallbackHandler(
//            ITelegramBotClient bot,
//            IReportService reportService)
//        {
//            _bot = bot;
//            _reportService = reportService;
//        }

//        public bool CanHandle(string data)
//            => data.StartsWith("report_action:");

//        public async Task HandleAsync(CallbackQuery callback)
//        {
//            await _bot.AnswerCallbackQueryAsync(callback.Id);

//            var parts = callback.Data!.Split(':');

//            var action = parts[1];
//            var reportType = int.Parse(parts[2]);
//            var projectId = Guid.Parse(parts[3]);

//            if (action == "generate")
//            {
//                await _reportService.GenerateNowAsync(projectId, reportType);

//                await _bot.EditMessageTextAsync(
//                    callback.Message!.Chat.Id,
//                    callback.Message.MessageId,
//                    "✅ تم إرسال طلب إنشاء التقرير إلى النظام.");
//            }
//            else
//            {
//                var keyboard = new InlineKeyboardMarkup(new[]
//                {
//                    new[]
//                    {
//                        InlineKeyboardButton.WithCallbackData(
//                            "كل يوم",
//                            $"schedule_time:daily:{reportType}:{projectId}")
//                    },

//                    new[]
//                    {
//                        InlineKeyboardButton.WithCallbackData(
//                            "كل أسبوع",
//                            $"schedule_time:weekly:{reportType}:{projectId}")
//                    },

//                    new[]
//                    {
//                        InlineKeyboardButton.WithCallbackData(
//                            "بعد انتهاء المشروع",
//                            $"schedule_time:followup:{reportType}:{projectId}")
//                    }
//                });

//                await _bot.EditMessageTextAsync(
//                    callback.Message!.Chat.Id,
//                    callback.Message.MessageId,
//                    "اختر موعد تشغيل التقرير:",
//                    replyMarkup: keyboard);
//            }
//        }
//    }
//}