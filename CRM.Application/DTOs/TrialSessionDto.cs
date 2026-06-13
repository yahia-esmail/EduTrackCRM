namespace CRM.Application.DTOs;

using System;
using CRM.Domain.Enums;

public class TrialSessionCreateDto
{
    public Guid LeadId { get; set; }
    public Guid TeacherId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public class TrialSessionAttendanceDto
{
    public Guid TrialSessionId { get; set; }
    public TrialSessionStatus Status { get; set; }
    public string? Notes { get; set; }
}
