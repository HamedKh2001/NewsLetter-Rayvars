using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SSO.Domain.Entities;
using SSO.Infrastructure.Persistence;
using SSO.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SSO.Infrastructure.UnitTests.Repositories
{
    public class UserLoginRepositoryTests
    {
        [Fact]
        public async Task CreateAsync_ShouldCreateUserLoginAndReturnIt()
        {
            // Arrange
            (var context, var repository) = GetContextAndUserLoginRepository();
            var userLogin = new UserLogin
            {
                UserId = 1,
                IpAddress = "127.0.0.1",
                ExtraInfo = "Test UserLogin",
                CreatedDate = DateTime.Now
            };

            // Act
            var result = await repository.CreateAsync(userLogin, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBe(0);
            result.UserId.Should().Be(userLogin.UserId);
            result.IpAddress.Should().Be(userLogin.IpAddress);
            result.ExtraInfo.Should().Be(userLogin.ExtraInfo);
            result.CreatedDate.Should().Be(userLogin.CreatedDate);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnPaginatedResultOfUserLogins()
        {
            // Arrange
            (var context, var repository) = GetContextAndUserLoginRepository();
            var user = new User
            {
                Id = 1,
                FirstName = "Alice",
                UserName = "UserName",
                Password = "Password",
                LastName = "LastName",
            };
            var userLogins = new List<UserLogin>
            {
                new UserLogin { UserId = 1, IpAddress = "127.0.0.1", ExtraInfo = "Test UserLogin 1", CreatedDate = DateTime.Now ,User=user},
                new UserLogin { UserId = 1, IpAddress = "127.0.0.2", ExtraInfo = "Test UserLogin 2", CreatedDate = DateTime.Now ,User=user},
                new UserLogin { UserId = 1, IpAddress = "127.0.0.3", ExtraInfo = "Test UserLogin 3", CreatedDate = DateTime.Now ,User=user}
            };

            await context.UserLogins.AddRangeAsync(userLogins);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAsync(1, 1, 2, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Pagination.TotalCount.Should().Be(3);
            result.Pagination.PageSize.Should().Be(2);
            result.Pagination.CurrentPage.Should().Be(1);
            result.Items.Should().HaveCount(2);
        }


        private (SSODbContext dbContext, UserLoginRepository userLoginRepository) GetContextAndUserLoginRepository()
        {
            var options = new DbContextOptionsBuilder<SSODbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new SSODbContext(options);
            var repository = new UserLoginRepository(context);
            return (context, repository);
        }
    }
}