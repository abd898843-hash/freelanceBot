using Freelance_bot.Application.IServieces;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Extensions.Constants;
using TelegramBot.Handlers.Interface;
using TelegramBot.Services.Keyboards;

namespace TelegramBot.Handlers.Commands;

public class TasksCommandHandler : ICommandHandler
{
    private readonly ITelegramBotClient _bot;
    private readonly IProjectService _projectService;
    private readonly KeyboardFactory _keyboards;

    public TasksCommandHandler(
        ITelegramBotClient bot,
        IProjectService projectService,
        KeyboardFactory keyboards)
    {
        _bot = bot;
        _projectService = projectService;
        _keyboards = keyboards;
    }

    public bool CanHandle(string input)
    {
        Console.WriteLine($"TASK INPUT = [{input}]");

        return input.Contains("Tasks");
    }
    public async Task HandleAsync(Message message)
    {
        if (message.From is null)
            return;

        var telegramId = message.From.Id;

        var projects =
            (await _projectService
                .GetByTelegramIdAsync(telegramId))
            .ToList();

        if (!projects.Any())
        {
            await _bot.SendTextMessageAsync(
                message.Chat.Id,
                "📭 مفيش مشاريع عندك حالياً.");
            return;
        }

        var inline =
            await _keyboards
                .GetProjectsTasksInlineAsync(
                    telegramId);

        await _bot.SendTextMessageAsync(
            message.Chat.Id,
            "📋 اختر مشروع لعرض التاسكات:",
            replyMarkup: inline);
    }
}