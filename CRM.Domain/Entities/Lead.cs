using CRM.Domain.Enums;
using CRM.Domain.Entities;

namespace CRM.Domain.Entities;

public class Lead
{
    public int Id { get; set; }
    
    // Required 15 fields
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public int? StudentAge { get; set; }
    public string GuardianName { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string CurrentLevel { get; set; } = string.Empty;
    public LeadSource Source { get; set; }
    public string CampaignName { get; set; } = string.Empty;
    public string AdSetName { get; set; } = string.Empty;
    public string AdName { get; set; } = string.Empty;
    public DateTime EntryTimestamp { get; set; } = DateTime.UtcNow;

    // Assignment & Pipeline
    public LeadStage Stage { get; set; } = LeadStage.NewLead;
    public string? AssignedUserId { get; set; }
    public AppUser? AssignedUser { get; set; }

    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastSeenAt { get; set; }
    // Navigation collections
    public ICollection<LeadNote> LeadNotes { get; set; } = new List<LeadNote>();
    public ICollection<LeadTag> LeadTags { get; set; } = new List<LeadTag>();
    public ICollection<FollowUp> FollowUps { get; set; } = new List<FollowUp>();
    public ICollection<Student> Students { get; set; } = new List<Student>();
}
