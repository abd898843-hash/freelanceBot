using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Freelance_bot.Application.Feature.Automation
{
    public class GenerateTasksPayload
    {
        public Guid ProjectId { get; set; } // تم تحويلها لـ Guid لتتوافق مع نظام الـ EntityId الخاص بك
        public string ProjectTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? Deadline { get; set; }
        public decimal? Budget { get; set; }
    }
}
