namespace CRM.Application.DTOs;

using System;
using CRM.Domain.Enums;

public class StudentDto
{
    public Guid Id { get; set; }
    public Guid LeadId { get; set; }
    public Guid? TeacherId { get; set; }
    public int WeeklySessions { get; set; }
    public DateTime StartDate { get; set; }
    public StudentStatus Status { get; set; }
    public string? GuardianName { get; set; }
    public string? GuardianPhone { get; set; }
    public string? Notes { get; set; }
}
