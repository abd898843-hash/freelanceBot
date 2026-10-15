using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Freelance_Bot.Domain.Entity
{
    public class AppEvent : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }

        [Required, MaxLength(100)]
        public string EntityType { get; set; } = string.Empty; 
        public Guid? EntityId { get; set; }

        [Required, MaxLength(100)]
        public string EventName { get; set; } = string.Empty; 

        public string? Payload { get; set; } 
        public string? ProcessedBy { get; set; } 
        public bool IsProcessed { get; set; } = false;
        public DateTime? ProcessedAt { get; set; }

        // Navigation
        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;
    }
}
