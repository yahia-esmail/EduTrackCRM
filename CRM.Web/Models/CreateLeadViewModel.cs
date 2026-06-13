using System.ComponentModel.DataAnnotations;
using CRM.Domain.Enums;

namespace CRM.Web.Models;

public class CreateLeadViewModel
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Country { get; set; } = string.Empty;

    [Required]
    public string City { get; set; } = string.Empty;

    [Required]
    public string Language { get; set; } = string.Empty;

    [Range(1, 100)]
    public int? StudentAge { get; set; }

    public string GuardianName { get; set; } = string.Empty;

    [Required]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string CurrentLevel { get; set; } = string.Empty;

    [Required]
    public LeadSource Source { get; set; } = LeadSource.Manual;

    public string CampaignName { get; set; } = string.Empty;
    public string AdSetName { get; set; } = string.Empty;
    public string AdName { get; set; } = string.Empty;
}
