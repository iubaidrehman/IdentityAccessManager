using IdentityAccessManager.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityAccessManager.Identity.Data;

public static class SeedData
{
    public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Create roles
        var roles = new[] { "Admin", "User", "Manager" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Create admin user
        var adminUser = new ApplicationUser
        {
            UserName = "admin@example.com",
            Email = "admin@example.com",
            FirstName = "Admin",
            LastName = "User",
            EmailConfirmed = true,
            IsActive = true
        };

        if (await userManager.FindByEmailAsync(adminUser.Email) == null)
        {
            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Create regular user
        var regularUser = new ApplicationUser
        {
            UserName = "user@example.com",
            Email = "user@example.com",
            FirstName = "Regular",
            LastName = "User",
            EmailConfirmed = true,
            IsActive = true
        };

        if (await userManager.FindByEmailAsync(regularUser.Email) == null)
        {
            var result = await userManager.CreateAsync(regularUser, "User123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(regularUser, "User");
            }
        }
    }
} 