using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Handlers.Interface;

namespace TelegramBot.Handlers.Commands
{
    public class AutoGenerateTasksCallbackHandler : ICallbackHandler
    {
        private readonly ITelegramBotClient _bot;

        public AutoGenerateTasksCallbackHandler(
            ITelegramBotClient bot)
        {
            _bot = bot;
        }

        public bool CanHandle(string data)
        {
            return data.StartsWith("external_tasks:");
        }

        public async Task HandleAsync(CallbackQuery callback)
        {
            if (callback.Message is null)
                return;

            var projectId =
                Guid.Parse(callback.Data!.Split(':')[1]);

            await _bot.SendTextMessageAsync(
                callback.Message.Chat.Id,
                $"🚀 Generate Tasks...\nProjectId = {projectId}");
        }
    }
}