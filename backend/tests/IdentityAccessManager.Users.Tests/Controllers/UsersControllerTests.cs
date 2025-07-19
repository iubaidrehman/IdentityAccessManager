using FluentAssertions;
using IdentityAccessManager.Shared.Contracts.Users;
using IdentityAccessManager.Users.Controllers;
using IdentityAccessManager.Users.Data;
using IdentityAccessManager.Users.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;

namespace IdentityAccessManager.Users.Tests.Controllers;

public class UsersControllerTests
{
    private readonly UsersDbContext _context;
    private readonly Mock<ILogger<UsersController>> _loggerMock;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        var options = new DbContextOptionsBuilder<UsersDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new UsersDbContext(options);
        _loggerMock = new Mock<ILogger<UsersController>>();
        _controller = new UsersController(_context, _loggerMock.Object);
    }

    [Fact]
    public async Task GetCurrentUser_WithValidUserId_ShouldReturnUser()
    {
        // Arrange
        var userId = "test-user-id";
        var user = new UserProfile
        {
            Id = userId,
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            IsActive = true
        };

        _context.UserProfiles.Add(user);
        await _context.SaveChangesAsync();

        var claims = new List<Claim>
        {
            new Claim("user_id", userId)
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        // Act
        var result = await _controller.GetCurrentUser();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var userDto = okResult.Value.Should().BeOfType<UserDto>().Subject;
        userDto.Id.Should().Be(userId);
        userDto.Email.Should().Be("test@example.com");
        userDto.FirstName.Should().Be("Test");
        userDto.LastName.Should().Be("User");
    }

    [Fact]
    public async Task GetCurrentUser_WithInvalidUserId_ShouldReturnNotFound()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("user_id", "invalid-user-id")
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        // Act
        var result = await _controller.GetCurrentUser();

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetCurrentUser_WithoutUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        var claims = new List<Claim>();
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        // Act
        var result = await _controller.GetCurrentUser();

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task UpdateCurrentUser_WithValidData_ShouldUpdateUser()
    {
        // Arrange
        var userId = "test-user-id";
        var user = new UserProfile
        {
            Id = userId,
            Email = "test@example.com",
            FirstName = "Old",
            LastName = "Name",
            IsActive = true
        };

        _context.UserProfiles.Add(user);
        await _context.SaveChangesAsync();

        var updateRequest = new UpdateUserRequest
        {
            FirstName = "New",
            LastName = "Name",
            PhoneNumber = "1234567890"
        };

        var claims = new List<Claim>
        {
            new Claim("user_id", userId)
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        // Act
        var result = await _controller.UpdateCurrentUser(updateRequest);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var userDto = okResult.Value.Should().BeOfType<UserDto>().Subject;
        userDto.FirstName.Should().Be("New");
        userDto.LastName.Should().Be("Name");
        userDto.PhoneNumber.Should().Be("1234567890");
    }

    [Fact]
    public async Task GetUsers_ShouldReturnAllActiveUsers()
    {
        // Arrange
        var users = new List<UserProfile>
        {
            new UserProfile { Id = "user1", Email = "user1@example.com", FirstName = "User", LastName = "One", IsActive = true },
            new UserProfile { Id = "user2", Email = "user2@example.com", FirstName = "User", LastName = "Two", IsActive = true },
            new UserProfile { Id = "user3", Email = "user3@example.com", FirstName = "User", LastName = "Three", IsActive = false }
        };

        _context.UserProfiles.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetUsers();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var userDtos = okResult.Value.Should().BeOfType<IEnumerable<UserDto>>().Subject;
        userDtos.Should().HaveCount(2); // Only active users
    }

    public void Dispose()
    {
        _context.Dispose();
    }
} 