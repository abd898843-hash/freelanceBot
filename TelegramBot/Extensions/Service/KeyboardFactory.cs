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

    // ====================================================
    // Main Menu
    // ====================================================

    public async Task<ReplyKeyboardMarkup> GetMainMenuAsync(long telegramId)
    {
        var projects = await _projectService.GetByTelegramIdAsync(telegramId);

        // بننشئ قائمة الأزرار من الصفر عشان نتجنب التكرار
        var buttons = new List<KeyboardButton[]>();

        // الصف الأول: My Projects و Tasks جنب بعض
        buttons.Add(new[]
        {
        new KeyboardButton(BotButtons.MyProjects),
        new KeyboardButton("📋 Tasks") // تأكد أن الاسم هنا هو الوحيد اللي بتستخدمه
    });

        // الصف الثاني: Create Project لوحده
        buttons.Add(new[]
        {
        new KeyboardButton(BotButtons.NewProject)
    });

        // باقي الأزرار (Dashboard, Workspace, إلخ) بتضاف فقط لو فيه مشاريع
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

    // ====================================================
    // Back Keyboard
    // ====================================================

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

    // ====================================================
    // Confirm Delete
    // ====================================================

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

    // ====================================================
    // Project Details
    // ====================================================

    public static InlineKeyboardMarkup GetProjectDetailInline(Guid projectId)
    {
        return new InlineKeyboardMarkup(
        new[]
        {
        new[] // أضفنا زر الكانبان هنا
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

    // ====================================================
    // Projects List
    // ====================================================

    // أضف هذه الميثود داخل كلاس KeyboardFactory في ملف KeyboardFactory.cs

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