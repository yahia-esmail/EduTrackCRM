using Microsoft.AspNetCore.Identity;
using CRM.Domain.Entities;
using CRM.Domain.Enums;

namespace CRM.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Seed Roles
        foreach (AppRole role in Enum.GetValues<AppRole>())
        {
            var roleName = role.ToString();
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Seed Admin User
        var adminEmail = "admin@onlineacademycrm.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        
        if (adminUser == null)
        {
            adminUser = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Administrator",
                Role = AppRole.Admin,
                IsActive = true,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123456");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, AppRole.Admin.ToString());
            }
        }
    }
}
