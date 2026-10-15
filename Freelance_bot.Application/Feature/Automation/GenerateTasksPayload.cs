using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Freelance_bot.Application.Feature.Automation
{
    namespace Freelance_bot.Application.Feature.Automation
    {
        public class GenerateTasksPayload
        {
            public Guid ProjectId { get; set; }
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string? Deadline { get; set; }
            public decimal? Budget { get; set; }
            public long ChatId { get; set; } 
        }
    }
}
