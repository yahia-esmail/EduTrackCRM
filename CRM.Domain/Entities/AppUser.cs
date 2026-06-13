using Microsoft.AspNetCore.Identity;
using CRM.Domain.Enums;

namespace CRM.Domain.Entities;

public class AppUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public AppRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
