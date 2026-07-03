using Freelance_bot.Application.Feature.Automation;

namespace TelegramBot.States;

public class ConversationContext
{
    public ConversationStep Step { get; set; }
        = ConversationStep.None;

    public Dictionary<string, string> Data { get; set; }
        = new();

    public Stack<string> NavigationHistory { get; set; }
        = new();

    public Guid? CreatedProjectId { get; set; }

    public Guid? CurrentProjectId { get; set; }

    // إضافة هذه الخاصية التي يستخدمها BotUpdateHandler للتحقق من حالة الإنشاء
    public bool IsProjectCreated { get; set; }
    public List<AutomationTaskDto>? TemporaryAutomationTasks { get; set; }
}