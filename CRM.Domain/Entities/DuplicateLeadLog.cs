using CRM.Domain.Enums;

namespace CRM.Domain.Entities;

public class DuplicateLeadLog
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public LeadSource Source { get; set; }
    public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;
    public int? ExistingLeadId { get; set; }
    public Lead? ExistingLead { get; set; }
}
