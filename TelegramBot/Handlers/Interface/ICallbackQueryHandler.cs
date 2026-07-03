using Telegram.Bot.Types;
using System.Threading.Tasks;

namespace TelegramBot.Handlers.Interface
{
    public interface ICallbackQueryHandler
    {
        bool CanHandle(string data);
        Task HandleAsync(CallbackQuery callback);
    }
}