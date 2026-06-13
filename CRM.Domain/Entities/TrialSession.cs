using System;
using CRM.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities
{
    public class TrialSession
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid LeadId { get; set; }

        [Required]
        public Guid TeacherId { get; set; }

        [Required]
        public DateTime ScheduledAt { get; set; }

        [Required]
        [MaxLength(100)]
        public string Subject { get; set; }

        [Required]
        [MaxLength(50)]
        public string Level { get; set; }

        [Required]
        public TrialSessionStatus Status { get; set; } = TrialSessionStatus.Scheduled;

        public string Notes { get; set; }

        // Navigation properties (optional, lazy loading may be enabled elsewhere)
        [ForeignKey(nameof(LeadId))]
        public Lead Lead { get; set; }

        [ForeignKey(nameof(TeacherId))]
        public AppUser Teacher { get; set; }
    }
}
