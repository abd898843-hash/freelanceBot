

using Freelance_bot.Application.Feature.Automation;
using Freelance_bot.Application.IServieces;
using Freelance_Bot.Domain.Entity;
using Freelance_Bot.Infrastruction.DB;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Handlers.Interface;
using TelegramBot.States;

namespace TelegramBot.Handlers.Commands
{
    public class SetupAutoTasksCallbackHandler : ICallbackHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly FreelancerDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConversationStateStore _stateStore;

        public SetupAutoTasksCallbackHandler(
            ITelegramBotClient botClient,
            FreelancerDbContext context,
            HttpClient httpClient,
            IConversationStateStore stateStore)
        {
            _botClient = botClient;
            _context = context;
            _httpClient = httpClient;
            _stateStore = stateStore;
        }

        public bool CanHandle(string data) => data == "setup_auto_tasks";
        public async Task HandleAsync(CallbackQuery callback)
        {
            long chatId = callback.Message!.Chat.Id;
            var ctx = _stateStore.Get(chatId);
            Guid projectId = ctx.CreatedProjectId ?? Guid.Empty;

            var project = await _context.projects.FirstOrDefaultAsync(p => p.Id == projectId);
            var userId = ctx.UserId; 

            
            var payload = new
            {
                projectId = projectId,
                chatId = chatId,
                title = project.Title,
                description = project.Description
            };

      


            var response = await _httpClient.PostAsJsonAsync("http://localhost:5678/webhook-test/create-project-tasks", payload);

            if (response.IsSuccessStatusCode)
                await _botClient.AnswerCallbackQueryAsync(callback.Id, "🚀 جاري توليد المهام بالذكاء الاصطناعي...");
            else
                await _botClient.SendTextMessageAsync(chatId, "❌ فشل الاتصال بخدمة الأتمتة.");
        }

        
        private async Task SendTasksToTelegram(long chatId, List<AutomationTaskDto> tasks, Guid projectId)
        {
            if (tasks == null || !tasks.Any())
            {
                await _botClient.SendTextMessageAsync(chatId, "⚠️ لم يتم العثور على مهام لتوليدها.");
                return;
            }

            string message = "✅ *تم توليد المهام بنجاح:*\n\n";
            foreach (var task in tasks)
            {
                message += $"🔹 *{task.Title}*\n{task.Notes}\n\n";
            }

            var kb = new InlineKeyboardMarkup(new[] {
        new[] { InlineKeyboardButton.WithCallbackData("➕ إضافة للداتابيز", $"save_tasks_{projectId}") },
        new[] { InlineKeyboardButton.WithCallbackData("⬅️ رجوع", $"project:{projectId}") }
    });

            await _botClient.SendTextMessageAsync(chatId, message, parseMode: ParseMode.Markdown, replyMarkup: kb);
        }
    }
}