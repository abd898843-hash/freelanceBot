using Freelance_Bot.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Freelance_Bot.Domain.Entity
{
     public class Notification : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }

        public NotificationChannel Channel { get; set; } = NotificationChannel.Telegram;
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public NotificationStatus Status { get; set; } = NotificationStatus.Pending;

        public string? RefEntityType { get; set; } 
        public Guid? RefEntityId { get; set; }

        public DateTime? SentAt { get; set; }
        public DateTime? ReadAt { get; set; }

        // Navigation
        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;
    }
}
