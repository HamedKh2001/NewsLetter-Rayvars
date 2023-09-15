using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SSO.Domain.Entities;
using SSO.Domain.Enums;
using SSO.Infrastructure.Persistence;
using SSO.Infrastructure.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SSO.Infrastructure.UnitTests.Repositories
{
    public class UserRepositoryTests
    {
        [Fact]
        public async Task GetAsync_WithSearchParam_ReturnsPaginatedResultWithFilteredUsers()
        {
            // Arrange
            (var context, var userRepository) = GetContextAndUserRepository();

            var user1 = new User
            {
                FirstName = "John",
                LastName = "Doe",
                Password = "Password",
                UserName = "John"
            };
            var user2 = new User
            {
                FirstName = "Jane",
                LastName = "Doe",
                Password = "Password",
                UserName = "Jane",
            };
            var user3 = new User
            {
                FirstName = "Bob",
                LastName = "Smith",
                Password = "Password",
                UserName = "Bob",
            };
            var user4 = new User
            {
                FirstName = "Alice",
                LastName = "Jones",
                Password = "Password",
                UserName = "Alice",
            };
            var user5 = new User
            {
                FirstName = "Charlie",
                LastName = "Brown",
                Password = "Password",
                UserName = "Charlie",
            };
            await context.Users.AddRangeAsync(user1, user2, user3, user4, user5);
            await context.SaveChangesAsync();

            // Act
            var result = await userRepository.GetAsync("Doe", 1, 2, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.Items.Should().NotContain(user3);
            result.Items.Should().NotContain(user4);
            result.Items.Should().NotContain(user5);
            result.Pagination.PageSize.Should().Be(2);
            result.Pagination.TotalCount.Should().Be(2);
        }

        [Fact]
        public async Task CreateAsync_AddsUserToContextAndReturnsUser()
        {
            // Arrange
            (var context, var userRepository) = GetContextAndUserRepository();

            var user = new User
            {
                FirstName = "John",
                LastName = "Doe",
                Password = "Password",
                UserName = "John",
            };

            // Act
            var result = await userRepository.CreateAsync(user, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(user);
            context.Users.Should().Contain(user);
        }

        [Fact]
        public async Task UpdateAsync_MarksUserAsModifiedAndSavesChanges()
        {
            // Arrange
            (var context, var userRepository) = GetContextAndUserRepository();

            var user = new User
            {
                FirstName = "John",
                LastName = "Doe",
                Password = "Password",
                UserName = "John",
            };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            user.FirstName = "Jane";

            // Act
            await userRepository.UpdateAsync(user, CancellationToken.None);

            // Assert
            context.Users.Should().Contain(user);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveUserFromDatabase()
        {
            // Arrange
            (var context, var userRepository) = GetContextAndUserRepository();
            var user = new User
            {
                FirstName = "Alice",
                LastName = "Adams",
                Password = "Password",
                UserName = "Alice",
            };
            context.Users.Add(user);
            context.SaveChanges();

            // Act
            await userRepository.DeleteAsync(user, CancellationToken.None);

            // Assert
            context.Users.Should().NotContain(user);
        }


        [Fact]
        public async Task GetUserByUserNameAndPassword_ReturnsCorrectUser()
        {
            // Arrange
            var userName = "user1";
            var password = "password";
            (var context, var userRepository) = GetContextAndUserRepository();
            SeedDatabase(context);
            // Act
            var user = await userRepository.GetUserWithRolesAsync(userName, password, CancellationToken.None);

            // Assert
            user.Should().NotBeNull();
            user.UserName.Should().Be(userName);
            user.Password.Should().Be(password);
        }

        [Fact]
        public async Task GetUserByPasswordAsync_ReturnsCorrectUser()
        {
            // Arrange
            var userId = 1;
            var password = "password";
            (var context, var userRepository) = GetContextAndUserRepository();
            SeedDatabase(context);

            // Act
            var user = await userRepository.GetUserByPasswordAsync(userId, password, CancellationToken.None);

            // Assert
            user.Should().NotBeNull();
            user.Id.Should().Be(userId);
            user.Password.Should().Be(password);
        }

        [Fact]
        public async Task GetUserWithRolesAsync_ReturnsCorrectUser()
        {
            // Arrange
            var userId = 1;
            (var context, var userRepository) = GetContextAndUserRepository();
            SeedDatabase(context);

            // Act
            var user = await userRepository.GetUserWithRolesAsync(userId, CancellationToken.None);

            // Assert
            user.Should().NotBeNull();
            user.Id.Should().Be(userId);
            user.Groups.Should().NotBeNull();
            user.Groups.Count.Should().Be(0);
        }


        private void SeedDatabase(SSODbContext context)
        {
            var user1 = new User
            {
                UserName = "user1",
                Password = "password",
                FirstName = "John",
                LastName = "Doe",
                Mobile = "1234567890",
                Gender = GenderType.Male,
                IsActive = true,
                IsDelete = false,
                CreatedDate = DateTime.Now
            };

            var user2 = new User
            {
                UserName = "user2",
                Password = "password",
                FirstName = "Jane",
                LastName = "Doe",
                Mobile = "1234567891",
                Gender = GenderType.Female,
                IsActive = true,
                IsDelete = false,
                CreatedDate = DateTime.Now
            };

            context.Users.AddRange(user1, user2);
            context.SaveChanges();
        }
        private (SSODbContext dbContext, UserRepository groupRepository) GetContextAndUserRepository()
        {
            var options = new DbContextOptionsBuilder<SSODbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new SSODbContext(options);
            var repository = new UserRepository(context);
            return (context, repository);
        }
    }
}
