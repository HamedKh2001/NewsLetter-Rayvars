using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SSO.Domain.Entities;
using SSO.Infrastructure.Persistence;
using SSO.Infrastructure.Repositories;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SSO.Infrastructure.UnitTests.Repositories
{
    public class RefreshTokenRepositoryTests
    {
        [Fact]
        public async Task AddAsync_ShouldAddRefreshTokenToDatabase()
        {
            // Arrange
            (var context, var refreshTokenRepository) = GetContextAndRefreshTokenRepository();
            var refreshToken = new RefreshToken
            {
                UserId = 1,
                Token = "1234567890",
                CreatedDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddMinutes(30)
            };

            // Act
            await refreshTokenRepository.AddAsync(refreshToken, CancellationToken.None);

            // Assert
            context.RefreshTokens.Should().Contain(refreshToken);
        }

        [Fact]
        public async Task GetLatestOneAsync_ShouldReturnLatestRefreshTokenForUser()
        {
            // Arrange
            (var context, var refreshTokenRepository) = GetContextAndRefreshTokenRepository();
            var userRefreshToken1 = new RefreshToken
            {
                Id = 1,
                UserId = 1,
                Token = "1234567890",
                CreatedDate = DateTime.UtcNow.AddMinutes(-120),
                ExpirationDate = DateTime.UtcNow.AddMinutes(30)
            };
            var userRefreshToken2 = new RefreshToken
            {
                Id = 2,
                UserId = 1,
                Token = "0987654321",
                CreatedDate = DateTime.UtcNow.AddMinutes(-30),
                ExpirationDate = DateTime.UtcNow.AddMinutes(-30)
            };
            var userRefreshToken3 = new RefreshToken
            {
                Id = 3,
                UserId = 2,
                Token = "abcdefghij",
                CreatedDate = DateTime.UtcNow.AddMinutes(-10),
                ExpirationDate = DateTime.UtcNow.AddMinutes(30)
            };
            await context.RefreshTokens.AddRangeAsync(userRefreshToken1, userRefreshToken2, userRefreshToken3);
            await context.SaveChangesAsync();

            // Act
            var latestRefreshToken = await refreshTokenRepository.GetLatestOneAsync(2, CancellationToken.None);

            // Assert
            latestRefreshToken.Should().Be(userRefreshToken3);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateRefreshTokenInDatabase()
        {
            // Arrange
            (var context, var refreshTokenRepository) = GetContextAndRefreshTokenRepository();
            var refreshToken = new RefreshToken
            {
                UserId = 1,
                Token = "1234567890",
                CreatedDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddMinutes(30)
            };
            await context.RefreshTokens.AddAsync(refreshToken);
            await context.SaveChangesAsync();
            refreshToken.Token = "0987654321";

            // Act
            await refreshTokenRepository.UpdateAsync(refreshToken, CancellationToken.None);

            // Assert
            context.Entry(refreshToken).State.Should().Be(EntityState.Unchanged);
            context.RefreshTokens.Single().Token.Should().Be("0987654321");
        }


        private (SSODbContext dbContext, RefreshTokenRepository refreshTokenRepository) GetContextAndRefreshTokenRepository()
        {
            var options = new DbContextOptionsBuilder<SSODbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new SSODbContext(options);
            var repository = new RefreshTokenRepository(context);
            return (context, repository);
        }
    }
}
