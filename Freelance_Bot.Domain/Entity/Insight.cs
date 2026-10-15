using Freelance_Bot.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Freelance_Bot.Domain.Entity
{
    public class Insight : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }
        public Guid? ProjectId { get; set; }

        public InsightCategory Category { get; set; }
        public InsightSeverity Severity { get; set; } = InsightSeverity.Info;

        [Required]
        public string Message { get; set; } = string.Empty;
        public string? ActionHint { get; set; }

        public bool IsDismissed { get; set; } = false;
        public string TriggerSource { get; set; } = "cron"; 
        public DateTime? ExpiresAt { get; set; }

        // Navigation
        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        [ForeignKey(nameof(ProjectId))]
        public Project? Project { get; set; }
    }
}
