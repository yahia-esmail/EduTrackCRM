using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

public class FollowUp
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid LeadId { get; set; }

    [Required]
    public DateTime ScheduledAt { get; set; }

    public DateTime? ReminderDate { get; set; }

    // Reminder channels
    public bool EmailReminder { get; set; } = true;
    public bool InAppReminder { get; set; } = true;

    public bool Sent { get; set; } = false;

    // Navigation
    [ForeignKey(nameof(LeadId))]
    public Lead Lead { get; set; }
}
