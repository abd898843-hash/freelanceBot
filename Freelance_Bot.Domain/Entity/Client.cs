using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Freelance_Bot.Domain.Entity
{
    public class Client : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Company { get; set; }
        public string Status { get; set; } = "active"; 
        public DateTime? LastContactedAt { get; set; }

        // Navigation
        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;
        public ICollection<Project> Projects { get; set; } = [];
    }

}
