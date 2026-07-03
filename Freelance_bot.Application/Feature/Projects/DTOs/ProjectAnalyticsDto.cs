using Freelance_Bot.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Freelance_bot.Application.Feature.Projects.DTOs
{
    public class ProjectAnalyticsDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? ClientName { get; set; }

    public ProjectStatus Status { get; set; }

    public int ProgressPct { get; set; }

    public DateTime? Deadline { get; set; }

    public int DaysUntilDeadline { get; set; }

    public int TotalTasks { get; set; }

    public int CompletedTasks { get; set; }

    public int OverdueTasks { get; set; }

    public DateTime LastActivity { get; set; }
}
}
