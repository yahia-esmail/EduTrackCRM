namespace CRM.Domain.Entities;

public class LeadAssignmentLog
{
    public int Id { get; set; }
    public int LeadId { get; set; }
    public Lead? Lead { get; set; }

    public string? FromUserId { get; set; }
    public AppUser? FromUser { get; set; }

    public string? ToUserId { get; set; }
    public AppUser? ToUser { get; set; }

    public string AssignedByUsername { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
