using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Freelance_bot.Application.Feature.Automation
{
    public class CreateTasksFromAutomationRequest
    {
        public Guid UserId { get; set; } // تم إضافتها هنا
        public Guid ProjectId { get; set; }
        public List<TaskItemDto> Tasks { get; set; } = new List<TaskItemDto>();
    }

    public class AutomationTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Notes { get; set; }
        // ممكن تضيف Priority أو DueDate لو الـ n8n بيبعتهم
    }
    public class TaskItemDto
    {
        public string Title { get; set; }
        public string Notes { get; set; }
        public int Priority { get; set; } // مثلاً: 1 = Low, 2 = Medium, 3 = High
        public DateTime? DueDate { get; set; }
    }
}
