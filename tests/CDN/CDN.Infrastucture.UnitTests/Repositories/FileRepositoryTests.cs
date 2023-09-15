using CDN.Application.Contracts.Persistence;
using CDN.Domain.Entities;
using CDN.Infrastructure.Persistence;
using CDN.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CDN.Infrastucture.UnitTests.Repositories
{
    public class FileRepositoryTests
    {
        [Fact]
        public async Task GetAsync_WithValidId_ReturnsFile()
        {
            // Arrange
            (var context, var repository) = GetContextAndUserRepository();
            var newsLetter = new NewsLetter { Id = 1, FileName = "test.txt" };
            context.NewsLetters.Add(newsLetter);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.FileName.Should().Be("test.txt");
        }

        [Fact]
        public async Task CreateAsync_WithValidFile_CreatesAndReturnsFile()
        {
            // Arrange
            (var context, var repository) = GetContextAndUserRepository();
            var newsLetter = new NewsLetter { FileName = "test.txt" };

            // Act
            var result = await repository.CreateAsync(newsLetter);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBe(0);
            result.FileName.Should().Be("test.txt");
        }

        [Fact]
        public async Task GetWithCategory_WithValidId_ReturnsFileWithCategory()
        {
            // Arrange
            (var context, var repository) = GetContextAndUserRepository();
            var category = new Category { Id = 1, Title = "Test Category" };
            var newsLetter = new NewsLetter { Id = 1, FileName = "test.txt", CategoryId = category.Id, Category = category };
            context.NewsLetters.Add(newsLetter);
            await context.SaveChangesAsync();

            // Act
            var result = repository.GetWithCategory(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.FileName.Should().Be("test.txt");
            result.Category.Should().NotBeNull();
            result.Category.Id.Should().Be(1);
            result.Category.Title.Should().Be("Test Category");
        }


        private (CDNDbContext dbContext, INewsLetterRepository _newsLetterRepository) GetContextAndUserRepository()
        {
            var options = new DbContextOptionsBuilder<CDNDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new CDNDbContext(options);
            var repository = new NewsLetterRepository(context);
            return (context, repository);
        }
    }
}
