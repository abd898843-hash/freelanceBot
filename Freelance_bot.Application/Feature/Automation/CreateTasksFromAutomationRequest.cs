using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Freelance_bot.Application.Feature.Automation
{
    public class CreateTasksFromAutomationRequest
    {
        public Guid ProjectId { get; set; }
        
        public List<AutomationTaskDto> Tasks { get; set; }
    }
    public class AutomationTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Notes { get; set; }
        
    }
    //public class TaskItemDto
    //{
    //    public string Title { get; set; }
    //    public string Notes { get; set; }
    //    public int Priority { get; set; } // مثلاً: 1 = Low, 2 = Medium, 3 = High
    //    public DateTime? DueDate { get; set; }
    //}
    public class AutomationResultDto
    {
        public bool Success { get; set; }
        public List<TaskItemDto> Tasks { get; set; }
    }

    public class TaskItemDto
    {
        public string Title { get; set; }
        public string Notes { get; set; }
    }
}
