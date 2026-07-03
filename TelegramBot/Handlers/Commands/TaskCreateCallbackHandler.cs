using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Handlers.Interface;
using TelegramBot.Navigation;

namespace TelegramBot.Handlers.Callbacks
{
    public class TaskCreateCallbackHandler : ICallbackHandler
    {
        private readonly ITelegramBotClient _bot;
        private readonly IUserNavigationStore _navigationStore;

        public TaskCreateCallbackHandler(ITelegramBotClient bot, IUserNavigationStore navigationStore)
        {
            _bot = bot;
            _navigationStore = navigationStore;
        }

        public bool CanHandle(string data) => data.StartsWith("task_create:");

        public async Task HandleAsync(CallbackQuery callback)
        {
            if (callback.Message is null) return;

            var projectId = Guid.Parse(callback.Data!.Split(':')[1]);
            var chatSideId = callback.Message.Chat.Id;

            // ✅ الحل الصحيح: التوجيه لـ ProjectDetail وتمرير معرّف المشروع
            _navigationStore.GoTo(chatSideId, NavigationScreen.ProjectDetail, projectId);

            // توجيه المستخدم برسالة
            await _bot.SendTextMessageAsync(
                chatSideId,
                "📝 تمام، أرسل الآن عنوان أو اسم التاسك الجديدة اللي حابب تضيفها للمشروع:"
            );
        }
    }
}