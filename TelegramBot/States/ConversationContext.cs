using Freelance_bot.Application.Feature.Automation;

namespace TelegramBot.States
{
    public class ConversationContext
    {
        public ConversationStep Step { get; set; } = ConversationStep.None;

        public Dictionary<string, string> Data { get; set; } = new();

        public Stack<string> NavigationHistory { get; set; } = new();

        // إضافة UserId هنا
        public Guid? UserId { get; set; }

        public Guid? CreatedProjectId { get; set; }

        public Guid? CurrentProjectId { get; set; }

        public bool IsProjectCreated { get; set; }

        public List<AutomationTaskDto>? TemporaryAutomationTasks { get; set; }
    }
}