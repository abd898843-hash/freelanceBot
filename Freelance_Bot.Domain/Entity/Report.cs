using Freelance_Bot.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Freelance_Bot.Domain.Entity
{
    public class Report : BaseEntity
    {
        [Required]
        public Guid ProjectId { get; set; }
        public Guid? GeneratedBy { get; set; } 

        public ReportType Type { get; set; } = ReportType.Manual;
        //public ReportStatus Status { get; set; } = ReportStatus.Pending;

        public string? AiSummary { get; set; }
        public string? AiSuggestions { get; set; }
        public string? RawData { get; set; } 

        public string? SentVia { get; set; } 
        public DateTime? SentAt { get; set; }
        public DateTime? GeneratedAt { get; set; }

        // Navigation
        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; } = null!;
    }
}
