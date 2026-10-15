using Freelance_bot.Application.IServieces;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Extensions.Constants;

namespace TelegramBot.Services.Keyboards;

public class KeyboardFactory
{
    private readonly IProjectService _projectService;

    public KeyboardFactory(
        IProjectService projectService)
    {
        _projectService = projectService;
    }

    public async Task<ReplyKeyboardMarkup> GetMainMenuAsync(long telegramId)
    {
        var projects = await _projectService.GetByTelegramIdAsync(telegramId);

   
        var buttons = new List<KeyboardButton[]>();

        
        buttons.Add(new[]
        {
        new KeyboardButton(BotButtons.MyProjects),
        new KeyboardButton("📋 Tasks") 
    });

      
        buttons.Add(new[]
        {
        new KeyboardButton(BotButtons.NewProject)
    });

   
        if (projects.Any())
        {
            buttons.Add(new[]
            {
            new KeyboardButton(BotButtons.Dashboard),
            new KeyboardButton(BotButtons.Workspace)
        });

            buttons.Add(new[]
            {
            new KeyboardButton(BotButtons.Reports),
            new KeyboardButton(BotButtons.DeleteProject)
        });
        }

        buttons.Add(new[] { new KeyboardButton(BotButtons.Help) });

        return new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true };
    }

    
    public static ReplyKeyboardMarkup
        GetBackKeyboard()
    {
        return new ReplyKeyboardMarkup(
            new[]
            {
                new[]
                {
                    new KeyboardButton(
                        BotButtons.Back),

                    new KeyboardButton(
                        BotButtons.Home)
                }
            })
        {
            ResizeKeyboard = true
        };
    }

    
    public static InlineKeyboardMarkup
        GetConfirmDeleteInline(
            Guid projectId)
    {
        return new InlineKeyboardMarkup(
        new[]
        {
            new[]
            {
                InlineKeyboardButton
                .WithCallbackData(
                    "✅ نعم",
                    $"confirm_delete:{projectId}"),

                InlineKeyboardButton
                .WithCallbackData(
                    "❌ إلغاء",
                    "cancel")
            }
        });
    }


    public static InlineKeyboardMarkup GetProjectDetailInline(Guid projectId)
    {
        return new InlineKeyboardMarkup(
        new[]
        {
        new[] 
        {
            InlineKeyboardButton.WithCallbackData("📊 Kanban Board", $"kanban:{projectId}")
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("📊 Dashboard", $"dashboard:{projectId}")
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("👥 Workspace", $"workspace:{projectId}")
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("📄 Reports", $"reports:{projectId}")
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("🗑 Delete", $"delete_project:{projectId}")
        }
        });
    }

  

    public async Task<InlineKeyboardMarkup> GetProjectsTasksInlineAsync(long telegramId)
    {
        var projects = await _projectService.GetByTelegramIdAsync(telegramId);
        var buttons = new List<InlineKeyboardButton[]>();

        foreach (var p in projects)
        {
            buttons.Add(new[]
            {
            InlineKeyboardButton.WithCallbackData($"📋 {p.Title}", $"tasks_project:{p.Id}"),
            InlineKeyboardButton.WithCallbackData("📊 Board", $"kanban:{p.Id}")
        });
        }

        return new InlineKeyboardMarkup(buttons);
    }
    public async Task<InlineKeyboardMarkup>
        GetProjectsInlineAsync(
            long telegramId)
    {
        var projects =
            await _projectService
            .GetByTelegramIdAsync(
                telegramId);

        var buttons =
            projects
            .Select(p =>
                new[]
                {
                    InlineKeyboardButton
                    .WithCallbackData(
                        $"📁 {p.Title}",
                        $"project:{p.Id}")
                })
            .ToArray();

        return new InlineKeyboardMarkup(
            buttons);
    }
}