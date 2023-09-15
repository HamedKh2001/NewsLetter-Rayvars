using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common;
using SSO.Domain.Entities;
using SSO.Infrastructure.Persistence;
using SSO.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MyProject.Tests.Repositories
{
    public class GroupRepositoryTests
    {
        [Fact]
        public async Task GetAsync_WithCaption_ReturnsFilteredResults()
        {
            // Arrange
            (var context, var groupRepository) = GetContextAndGroupRepository();
            await groupRepository.CreateAsync(new Group { Caption = "Group 1" }, CancellationToken.None);
            await groupRepository.CreateAsync(new Group { Caption = "Group 2" }, CancellationToken.None);
            await groupRepository.CreateAsync(new Group { Caption = "Another Group 1" }, CancellationToken.None);

            // Act
            var result = await groupRepository.GetAsync("Group 1", 1, 10, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.Items.All(g => g.Caption.Contains("Group 1")).Should().BeTrue();
        }

        [Fact]
        public async Task GetAsync_WithoutCaption_ReturnsAllResults()
        {
            // Arrange
            (var context, var groupRepository) = GetContextAndGroupRepository();
            await groupRepository.CreateAsync(new Group { Caption = "Group 1" }, CancellationToken.None);
            await groupRepository.CreateAsync(new Group { Caption = "Group 2" }, CancellationToken.None);

            // Act
            var result = await groupRepository.GetAsync(CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task CreateAsync_AddsNewGroupToDatabase()
        {
            // Arrange
            (var context, var groupRepository) = GetContextAndGroupRepository();
            var group = new Group { Caption = "New Group" };

            // Act
            var result = await groupRepository.CreateAsync(group, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBe(0);
            context.Groups.Should().Contain(g => g.Id == result.Id && g.Caption == "New Group");
        }

        [Fact]
        public async Task UpdateAsync_ModifiesExistingGroupInDatabase()
        {
            // Arrange
            (var context, var groupRepository) = GetContextAndGroupRepository();
            var group = new Group { Caption = "Group 1" };
            await groupRepository.CreateAsync(group, CancellationToken.None);
            group.Caption = "Updated Group";

            // Act
            await groupRepository.UpdateAsync(group, CancellationToken.None);

            // Assert
            var result = await groupRepository.GetAsync(group.Id, CancellationToken.None);
            result.Should().NotBeNull();
            result.Caption.Should().Be("Updated Group");
        }

        [Fact]
        public async Task GetAsync_ReturnsPaginatedResult_WithMatchingCaption()
        {
            // Arrange
            (var context, var groupRepository) = GetContextAndGroupRepository();
            context.Groups.Add(new Group { Caption = "Group A" });
            context.Groups.Add(new Group { Caption = "Group B" });
            context.Groups.Add(new Group { Caption = "Group C" });
            context.SaveChanges();

            // Act
            var result = await groupRepository.GetAsync("roup", 1, 2, CancellationToken.None);

            // Assert
            result.Should().BeOfType<PaginatedResult<Group>>();
            result.Items.Should().HaveCount(2);
            result.Items.Should().Contain(g => g.Caption == "Group A");
            result.Items.Should().Contain(g => g.Caption == "Group B");
            result.Items.Count.Should().Be(2);
            result.Pagination.PageSize.Should().Be(2);
        }

        [Fact]
        public async Task GetAsync_ReturnsNull_WhenIdNotFound()
        {
            // Arrange
            (var context, var groupRepository) = GetContextAndGroupRepository();
            context.Groups.Add(new Group { Id = 1, Caption = "Group A" });
            context.SaveChanges();

            // Act
            var result = await groupRepository.GetAsync(2, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdsAsync_ShouldReturnGroupsByIds()
        {
            // Arrange
            (var context, var groupRepository) = GetContextAndGroupRepository();
            var groups = new List<Group>
            {
            new Group { Id = 1, Caption = "Group 1" },
            new Group { Id = 2, Caption = "Group 2" },
            new Group { Id = 3, Caption = "Group 3" },
            };

            await context.Groups.AddRangeAsync(groups);
            await context.SaveChangesAsync();

            var ids = new List<int> { 1, 3 };

            // Act
            var result = await groupRepository.GetByIdsAsync(ids, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(g => g.Id == 1);
            result.Should().Contain(g => g.Id == 3);
        }

        [Fact]
        public async Task IsUniqueCaptionAsync_ShouldReturnTrue_WhenCaptionIsUnique()
        {
            // Arrange
            (var context, var groupRepository) = GetContextAndGroupRepository();
            var groups = new List<Group>
            {
            new Group { Caption = "Group 1" },
            new Group { Caption = "Group 2" },
            new Group { Caption = "Group 3" },
            };

            await context.Groups.AddRangeAsync(groups);
            await context.SaveChangesAsync();

            var caption = "Group 4";

            // Act
            var result = await groupRepository.IsUniqueCaptionAsync(caption, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsUniqueCaptionAsync_ShouldReturnFalse_WhenCaptionIsNotUnique()
        {
            // Arrange
            (var context, var groupRepository) = GetContextAndGroupRepository();
            var groups = new List<Group>
            {
            new Group { Caption = "Group 1" },
            new Group { Caption = "Group 2" },
            new Group { Caption = "Group 3" },
            };

            await context.Groups.AddRangeAsync(groups);
            await context.SaveChangesAsync();

            var caption = "Group 2";

            // Act
            var result = await groupRepository.IsUniqueCaptionAsync(caption, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task IsUniqueCaptionAsync_ShouldReturnTrue_WhenCaptionIsUniqueForDifferentGroup()
        {
            // Arrange
            (var context, var groupRepository) = GetContextAndGroupRepository();
            var groups = new List<Group>
            {
            new Group { Id = 1, Caption = "Group 1" },
            new Group { Id = 2, Caption = "Group 2" },
            new Group { Id = 3, Caption = "Group 3" },
            };

            await context.Groups.AddRangeAsync(groups);
            await context.SaveChangesAsync();

            var id = 2;
            var caption = "Group 4";

            // Act
            var result = await groupRepository.IsUniqueCaptionAsync(id, caption, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task GetWithRolesAsync_ReturnsGroupWithRoles()
        {
            // Arrange
            (var context, var groupRepository) = GetContextAndGroupRepository();
            var group = new Group
            {
                Caption = "Test Group",
                Roles = new List<Role>
                {
                    new Role { Title = "Test Role 1" },
                    new Role { Title = "Test Role 2" }
                }
            };

            context.Groups.Add(group);
            context.SaveChanges();

            // Act
            var result = await groupRepository.GetWithRolesAsync(1, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Caption.Should().Be("Test Group");
            result.Roles.Should().HaveCount(2);
            result.Roles.Should().Contain(r => r.Title == "Test Role 1");
            result.Roles.Should().Contain(r => r.Title == "Test Role 2");
        }

        //[Fact]
        //public async Task DeleteAsync_RemovesGroupFromDatabase()
        //{
        //    // Arrange
        //    using var context = new SSODbContext(_options);
        //    var repository = new GroupRepository(context);
        //    var group = new Group { Caption = "Group 1" };
        //    await repository.CreateAsync(group);

        //    // Act
        //    await repository.DeleteAsync(group);

        //    // Assert
        //    context.Groups.Should().NotContain(g => g.Id == group.Id);
        //} 


        private (SSODbContext dbContext, GroupRepository groupRepository) GetContextAndGroupRepository()
        {
            var options = new DbContextOptionsBuilder<SSODbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new SSODbContext(options);
            var repository = new GroupRepository(context);
            return (context, repository);
        }
    }
}

