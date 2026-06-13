using System;
using System.Collections.Generic;
using CRM.Domain.Enums;

namespace CRM.Domain.Entities;

public class Student
{
    public Guid Id { get; set; }
    public Guid LeadId { get; set; }
    public Guid TeacherId { get; set; }
    public int WeeklySessionCount { get; set; }
    public DateTime StartDate { get; set; }
    public string? GuardianName { get; set; }
    public string? GuardianPhone { get; set; }
    public StudentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Lead Lead { get; set; } = null!;
    public AppUser Teacher { get; set; } = null!;
    public ICollection<StudentStatusHistory> StatusHistories { get; set; } = new List<StudentStatusHistory>();
    public ICollection<StudentContactAttempt> ContactAttempts { get; set; } = new List<StudentContactAttempt>();
}
