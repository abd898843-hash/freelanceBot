//using Domain.Enums;
//using Freelance_Bot.Domain.Entity;
//using Freelance_Bot.Domain.Enum;

//namespace Domain.Entities;

//public class ReportSchedule : BaseEntity
//{
//    public Guid ProjectId { get; set; }

//    public Project Project { get; set; } = null!;

//    public ReportScheduleType ReportType { get; set; }

//    public DateTime NextRun { get; set; }

//    public DateTime? LastRun { get; set; }

//    public bool IsActive { get; set; } = true;

//    public long TelegramChatId { get; set; }
//    public ScheduleType ScheduleType { get; set; }
//}
//public class ReportHistory
//{
//    public Guid Id { get; set; }
//    public Guid ReportScheduleId { get; set; }
//    public ReportSchedule ReportSchedule { get; set; }
//    public DateTime SentAt { get; set; } = DateTime.UtcNow;
//    public ReportStatus Status { get; set; }
//    public string? AiResponse { get; set; }
//    public string? ErrorMessage { get; set; }
//}

//public enum ScheduleType { Daily, Weekly, FollowUp }