using FluentAssertions;
using IdentityAccessManager.Identity.Data;
using IdentityAccessManager.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Testcontainers.MsSql;
using Xunit;

namespace IdentityAccessManager.Integration.Tests;

public class IdentityServiceIntegrationTests : IAsyncDisposable
{
    private readonly MsSqlContainer _sqlServerContainer;
    private readonly IServiceProvider _serviceProvider;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public IdentityServiceIntegrationTests()
    {
        _sqlServerContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("YourStrong@Passw0rd")
            .WithName("test-sqlserver")
            .Build();

        // Start the container
        _sqlServerContainer.StartAsync().Wait();

        // Create service collection
        var services = new ServiceCollection();

        // Add DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(_sqlServerContainer.GetConnectionString()));

        // Add Identity
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        _serviceProvider = services.BuildServiceProvider();

        // Get services
        _userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        _roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Ensure database is created
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated();
    }

    [Fact]
    public async Task CreateUser_WithValidData_ShouldSucceed()
    {
        // Arrange
        var user = new ApplicationUser
        {
            UserName = "test@example.com",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            EmailConfirmed = true,
            IsActive = true
        };

        // Act
        var result = await _userManager.CreateAsync(user, "TestPassword123!");

        // Assert
        result.Succeeded.Should().BeTrue();
        user.Id.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CreateUser_WithInvalidPassword_ShouldFail()
    {
        // Arrange
        var user = new ApplicationUser
        {
            UserName = "test@example.com",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User"
        };

        // Act
        var result = await _userManager.CreateAsync(user, "weak");

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateRole_AndAssignToUser_ShouldSucceed()
    {
        // Arrange
        var role = new IdentityRole("TestRole");
        var user = new ApplicationUser
        {
            UserName = "test@example.com",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            EmailConfirmed = true
        };

        // Act
        var createRoleResult = await _roleManager.CreateAsync(role);
        var createUserResult = await _userManager.CreateAsync(user, "TestPassword123!");
        var addToRoleResult = await _userManager.AddToRoleAsync(user, "TestRole");

        // Assert
        createRoleResult.Succeeded.Should().BeTrue();
        createUserResult.Succeeded.Should().BeTrue();
        addToRoleResult.Succeeded.Should().BeTrue();

        var userRoles = await _userManager.GetRolesAsync(user);
        userRoles.Should().Contain("TestRole");
    }

    [Fact]
    public async Task AuthenticateUser_WithValidCredentials_ShouldSucceed()
    {
        // Arrange
        var user = new ApplicationUser
        {
            UserName = "test@example.com",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            EmailConfirmed = true
        };

        await _userManager.CreateAsync(user, "TestPassword123!");

        // Act
        var result = await _userManager.CheckPasswordAsync(user, "TestPassword123!");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task AuthenticateUser_WithInvalidCredentials_ShouldFail()
    {
        // Arrange
        var user = new ApplicationUser
        {
            UserName = "test@example.com",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            EmailConfirmed = true
        };

        await _userManager.CreateAsync(user, "TestPassword123!");

        // Act
        var result = await _userManager.CheckPasswordAsync(user, "WrongPassword");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateUser_ShouldPersistChanges()
    {
        // Arrange
        var user = new ApplicationUser
        {
            UserName = "test@example.com",
            Email = "test@example.com",
            FirstName = "Old",
            LastName = "Name",
            EmailConfirmed = true
        };

        await _userManager.CreateAsync(user, "TestPassword123!");

        // Act
        user.FirstName = "New";
        user.LastName = "Name";
        var result = await _userManager.UpdateAsync(user);

        // Assert
        result.Succeeded.Should().BeTrue();

        var updatedUser = await _userManager.FindByEmailAsync("test@example.com");
        updatedUser.Should().NotBeNull();
        updatedUser!.FirstName.Should().Be("New");
        updatedUser.LastName.Should().Be("Name");
    }

    public async ValueTask DisposeAsync()
    {
        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }

        await _sqlServerContainer.DisposeAsync();
    }
} 