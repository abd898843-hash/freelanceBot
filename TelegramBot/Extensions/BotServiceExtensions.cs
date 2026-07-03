using FreelanceOS.Bot.Handlers.Commands;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using TelegramBot.Extensions.Service;
using TelegramBot.Handlers;
using TelegramBot.Handlers.Callbacks; // تأكد من إضافة هذا النيم سبيس
using TelegramBot.Handlers.Commands;
using TelegramBot.Handlers.Interface;
using TelegramBot.Navigation;
using TelegramBot.Services;
using TelegramBot.Services.Keyboards;
using TelegramBot.States;

namespace TelegramBot.Extensions
{
    public static class BotServiceExtensions
    {
        public static IServiceCollection AddTelegramBot(this IServiceCollection services, string token)
        {
            // ── Singletons ──────────────────────────────────────────
            services.AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(token));
            services.AddSingleton<IConversationStateStore, ConversationStateStore>();
            services.AddSingleton<IUserNavigationStore, UserNavigationStore>();

            // ── Scoped ──────────────────────────────────────────────
            services.AddScoped<KeyboardFactory>();

            // ── Command Handlers ────────────────────────────────────
            services.AddScoped<ICommandHandler, StartCommandHandler>();
            services.AddScoped<ICommandHandler, NewProjectCommandHandler>();
            services.AddScoped<ICommandHandler, ProjectsCommandHandler>();
            services.AddScoped<ICommandHandler, DashboardCommandHandler>();
            services.AddScoped<ICommandHandler, ReportsCommandHandler>();
            services.AddScoped<ICommandHandler, WorkspaceCommandHandler>();
            services.AddScoped<ICommandHandler, HelpCommandHandler>();
            services.AddScoped<ICommandHandler, BackCommandHandler>();
            services.AddScoped<ICommandHandler, DeleteProjectCommandHandler>();

            // ── Callback Handlers (نظام الـ Kanban والتاسكات) ────────
            services.AddTransient<ICallbackHandler, DashboardCallbackHandler>();
            services.AddTransient<ICallbackHandler, TasksKanbanCallbackHandler>();
            services.AddTransient<ICallbackHandler, TaskMoveCallbackHandler>();
            services.AddTransient<ICallbackHandler, TaskDeleteCallbackHandler>();
            services.AddTransient<ICallbackHandler, TaskOpenCallbackHandler>();

            // ── Main router ─────────────────────────────────────────
            services.AddScoped<BotUpdateHandler>();

            return services;
        }
    }
}