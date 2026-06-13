using CRM.Domain.Enums;

namespace CRM.Application.DTOs;

public class LeadDto
{
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
    public DateTime? EntryTimestamp { get; set; }
}

public class MetaLeadWebhookModel
{
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
    public string Platform { get; set; } = "facebook"; // facebook or instagram
    public string CampaignName { get; set; } = string.Empty;
    public string AdSetName { get; set; } = string.Empty;
    public string AdName { get; set; } = string.Empty;
}

public class TikTokLeadWebhookModel
{
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
    public string CampaignName { get; set; } = string.Empty;
    public string AdSetName { get; set; } = string.Empty;
    public string AdName { get; set; } = string.Empty;
}
