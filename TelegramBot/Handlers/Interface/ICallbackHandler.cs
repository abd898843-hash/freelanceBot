using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    using Telegram.Bot.Types;

namespace TelegramBot.Handlers.Interface
{
    public interface ICallbackHandler
    {
        bool CanHandle(string data);
        Task HandleAsync(CallbackQuery callback);
    }
}

