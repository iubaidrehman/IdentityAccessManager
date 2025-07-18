using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityAccessManager.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IdentityAccessManager.Identity.Services;

public class ProfileService : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ProfileService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);
        if (user == null) return;

        var claims = new List<Claim>
        {
            new Claim("first_name", user.FirstName),
            new Claim("last_name", user.LastName),
            new Claim("email", user.Email ?? ""),
            new Claim("user_id", user.Id),
            new Claim("tenant_id", user.TenantId ?? "")
        };

        // Add roles
        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim("role", role));
        }

        context.IssuedClaims.AddRange(claims);
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);
        context.IsActive = user?.IsActive ?? false;
    }
} 