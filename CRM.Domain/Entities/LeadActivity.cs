using CRM.Domain.Enums;

namespace CRM.Domain.Entities;

public class LeadActivity
{
    public int Id { get; set; }
    public int LeadId { get; set; }
    public Lead? Lead { get; set; }
    
    public string ActorName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public LeadStage? PreviousStage { get; set; }
    public LeadStage? NewStage { get; set; }
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Metadata { get; set; } // Extra JSON values if needed
}
