using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Freelance_Bot.Domain.Entity
{
    public class TaskItem : BaseEntity
    {
        [Required]
        public Guid ProjectId { get; set; }
        public Guid? AssignedTo { get; set; }
        public Guid? ParentTaskId { get; set; }

        [Required, MaxLength(300)]
        public string Title { get; set; } = string.Empty;
        public string? Notes { get; set; }

        public Enum.TaskStatus Status { get; set; } = Enum.TaskStatus.Todo;
        public Enum.TaskPriority Priority { get; set; } = Enum.TaskPriority.Medium;
        public DateTime? DueDate { get; set; }
        public bool IsDefault { get; set; } = false; 
        public DateTime? CompletedAt { get; set; }

        // Navigation
        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; } = null!;

        [ForeignKey(nameof(ParentTaskId))]
        public TaskItem? ParentTask { get; set; }
        public ICollection<TaskItem> SubTasks { get; set; } = [];
    }
}
