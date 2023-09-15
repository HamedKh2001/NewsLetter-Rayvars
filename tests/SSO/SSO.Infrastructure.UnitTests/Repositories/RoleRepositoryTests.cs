using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SSO.Domain.Entities;
using SSO.Infrastructure.Persistence;
using SSO.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class RoleRepositoryTests
{
    [Fact]
    public async Task GetAsync_ShouldReturnListOfRoles()
    {
        // Arrange
        (var context, var roleRepository) = GetContextAndRoleRepository();
        await context.Roles.AddAsync(new Role { Title = "Role 1", DisplayTitle = "Role 1 DT" });
        await context.Roles.AddAsync(new Role { Title = "Role 2", DisplayTitle = "Role 2 DT" });
        await context.Roles.AddAsync(new Role { Title = "Role 3", DisplayTitle = "Role 3 DT" });
        await context.SaveChangesAsync();

        // Act
        var result = await roleRepository.GetAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result.Select(r => r.Title).Should().Contain(new[] { "Role 1", "Role 2", "Role 3" });
        result.Select(r => r.DisplayTitle).Should().Contain(new[] { "Role 1 DT", "Role 2 DT", "Role 3 DT" });
    }

    [Fact]
    public async Task GetAsync_WithId_ShouldReturnRole()
    {
        // Arrange
        (var context, var roleRepository) = GetContextAndRoleRepository();
        var role1 = await context.AddAsync(new Role { Title = "Role 1", DisplayTitle = "Role 1 DT" });
        await context.AddAsync(new Role { Title = "Role 2", DisplayTitle = "Role 2 DT" });
        await context.AddAsync(new Role { Title = "Role 3", DisplayTitle = "Role 3 DT" });

        // Act
        var result = await roleRepository.GetAsync(1, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Role 1");
        result.DisplayTitle.Should().Be("Role 1 DT");
    }

    [Fact]
    public async Task GetByRoleIdsAsync_ShouldReturnMatchingRoles()
    {
        // Arrange
        (var context, var roleRepository) = GetContextAndRoleRepository();
        var roles = new List<Role>
        {
            new Role { Id = 1, Title = "Role 1" },
            new Role { Id = 2, Title = "Role 2" },
            new Role { Id = 3, Title = "Role 3" },
            new Role { Id = 4, Title = "Role 4" },
            new Role { Id = 5, Title = "Role 5" }
        };
        context.Roles.AddRange(roles);
        await context.SaveChangesAsync();

        var ids = new List<int> { 1, 3, 5 };

        // Act
        var result = await roleRepository.GetByRoleIdsAsync(ids, CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result.Select(r => r.Id).Should().BeEquivalentTo(ids);
    }

    [Fact]
    public async Task GetWithGroupsAsync_ShouldReturnMatchingRole()
    {
        // Arrange
        (var context, var roleRepository) = GetContextAndRoleRepository();
        var role = new Role { Id = 1, Title = "Role 1" };
        role.Groups.Add(new Group { Id = 1, Caption = "Group 1" });
        role.Groups.Add(new Group { Id = 2, Caption = "Group 2" });
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        // Act
        var result = await roleRepository.GetWithGroupsAsync(role.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Groups.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetWithUsersAsync_ShouldReturnRoleWithUsers()
    {
        // Arrange
        (var context, var roleRepository) = GetContextAndRoleRepository();
        var role = new Role
        {
            Id = 1,
            DisplayTitle = "Admin",
            Title = "Title",
        };
        var group = new Group
        {
            Id = 1,
            Caption = "Admins"
        };
        var user1 = new User
        {
            Id = 1,
            FirstName = "Alice",
            UserName = "UserName",
            Password = "Password",
            LastName = "LastName",
        };
        var user2 = new User
        {
            Id = 2,
            FirstName = "Bob",
            UserName = "UserName",
            Password = "Password",
            LastName = "LastName",
        };
        group.Users.Add(user1);
        group.Users.Add(user2);
        role.Groups.Add(group);
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        // Act
        var result = await roleRepository.GetWithUsersAsync(1, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.DisplayTitle.Should().Be("Admin");
        result.Groups.Should().NotBeNull();
        result.Groups.Should().HaveCount(1);
        result.Groups.First().Caption.Should().Be("Admins");
        result.Groups.First().Users.Should().NotBeNull();
        result.Groups.First().Users.Should().HaveCount(2);
        result.Groups.First().Users.Should().Contain(u => u.FirstName == "Alice");
        result.Groups.First().Users.Should().Contain(u => u.FirstName == "Bob");
    }

    [Fact]
    public async Task IsUniqueDisplayTitleAsync_ShouldReturnTrue_WhenDisplayTitleIsUnique()
    {
        // Arrange
        (var context, var roleRepository) = GetContextAndRoleRepository();
        context.Roles.Add(new Role
        {
            Id = 1,
            DisplayTitle = "Admin",
            Title = "Title",
        });
        await context.SaveChangesAsync();

        // Act
        var result = await roleRepository.IsUniqueDisplayTitleAsync(0, "Manager", CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsUniqueDisplayTitleAsync_ShouldReturnFalse_WhenDisplayTitleIsNotUnique()
    {
        // Arrange
        (var context, var roleRepository) = GetContextAndRoleRepository();
        context.Roles.Add(new Role
        {
            Id = 1,
            DisplayTitle = "Admin",
            Title = "Title"
        });
        await context.SaveChangesAsync();

        // Act
        var result = await roleRepository.IsUniqueDisplayTitleAsync(0, "Admin", CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateRole()
    {
        // Arrange
        (var context, var roleRepository) = GetContextAndRoleRepository();
        var role = new Role
        {
            Id = 1,
            Title = "Admin"
        };
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        // Act
        role.Title = "Manager";
        await roleRepository.UpdateAsync(role, CancellationToken.None);
        var updatedRole = await roleRepository.GetAsync(1, CancellationToken.None);

        // Assert
        updatedRole.Should().NotBeNull();
        updatedRole.Id.Should().Be(1);
        updatedRole.Title.Should().Be("Manager");
    }

    private (SSODbContext dbContext, RoleRepository roleRepository) GetContextAndRoleRepository()
    {
        var options = new DbContextOptionsBuilder<SSODbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new SSODbContext(options);
        var repository = new RoleRepository(context);
        return (context, repository);
    }
}