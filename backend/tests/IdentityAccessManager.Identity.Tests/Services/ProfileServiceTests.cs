using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using FluentAssertions;
using IdentityAccessManager.Identity.Models;
using IdentityAccessManager.Identity.Services;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Security.Claims;

namespace IdentityAccessManager.Identity.Tests.Services;

public class ProfileServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
    private readonly ProfileService _profileService;

    public ProfileServiceTests()
    {
        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);

        var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
        _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
            roleStoreMock.Object, null, null, null, null);

        _profileService = new ProfileService(_userManagerMock.Object, _roleManagerMock.Object);
    }

    [Fact]
    public async Task GetProfileDataAsync_WithValidUser_ShouldAddCustomClaims()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = "test-user-id",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            TenantId = "tenant-1"
        };

        var roles = new List<string> { "User", "Admin" };
        var claims = new List<Claim>
        {
            new Claim("sub", user.Id),
            new Claim("email", user.Email)
        };

        var context = new ProfileDataRequestContext
        {
            Subject = new ClaimsPrincipal(new ClaimsIdentity(claims)),
            IssuedClaims = new List<Claim>()
        };

        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);
        _userManagerMock.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(roles);

        // Act
        await _profileService.GetProfileDataAsync(context);

        // Assert
        context.IssuedClaims.Should().Contain(c => c.Type == "first_name" && c.Value == "Test");
        context.IssuedClaims.Should().Contain(c => c.Type == "last_name" && c.Value == "User");
        context.IssuedClaims.Should().Contain(c => c.Type == "email" && c.Value == "test@example.com");
        context.IssuedClaims.Should().Contain(c => c.Type == "user_id" && c.Value == "test-user-id");
        context.IssuedClaims.Should().Contain(c => c.Type == "tenant_id" && c.Value == "tenant-1");
        context.IssuedClaims.Should().Contain(c => c.Type == "role" && c.Value == "User");
        context.IssuedClaims.Should().Contain(c => c.Type == "role" && c.Value == "Admin");
    }

    [Fact]
    public async Task GetProfileDataAsync_WithNullUser_ShouldNotAddClaims()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("sub", "test-user-id"),
            new Claim("email", "test@example.com")
        };

        var context = new ProfileDataRequestContext
        {
            Subject = new ClaimsPrincipal(new ClaimsIdentity(claims)),
            IssuedClaims = new List<Claim>()
        };

        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        await _profileService.GetProfileDataAsync(context);

        // Assert
        context.IssuedClaims.Should().BeEmpty();
    }

    [Fact]
    public async Task IsActiveAsync_WithActiveUser_ShouldReturnTrue()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = "test-user-id",
            IsActive = true
        };

        var claims = new List<Claim>
        {
            new Claim("sub", user.Id)
        };

        var context = new IsActiveContext
        {
            Subject = new ClaimsPrincipal(new ClaimsIdentity(claims))
        };

        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);

        // Act
        await _profileService.IsActiveAsync(context);

        // Assert
        context.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task IsActiveAsync_WithInactiveUser_ShouldReturnFalse()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = "test-user-id",
            IsActive = false
        };

        var claims = new List<Claim>
        {
            new Claim("sub", user.Id)
        };

        var context = new IsActiveContext
        {
            Subject = new ClaimsPrincipal(new ClaimsIdentity(claims))
        };

        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);

        // Act
        await _profileService.IsActiveAsync(context);

        // Assert
        context.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task IsActiveAsync_WithNullUser_ShouldReturnFalse()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("sub", "test-user-id")
        };

        var context = new IsActiveContext
        {
            Subject = new ClaimsPrincipal(new ClaimsIdentity(claims))
        };

        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        await _profileService.IsActiveAsync(context);

        // Assert
        context.IsActive.Should().BeFalse();
    }
} 