using CDN.Application.Contracts.Persistence;
using CDN.Domain.Entities;
using CDN.Infrastructure.Persistence;
using CDN.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CDN.Infrastucture.UnitTests.Repositories
{
    public class CategoryRepositoryTests
    {
        [Fact]
        public async Task GetAsync_WithExistingCategoryId_ShouldReturnCategory()
        {
            // Arrange
            (var context, var repository) = GetContextAndUserRepository();
            var expectedCategory = new Category { Id = 1, Title = "Test Category", IsActive = true };
            await context.Categories.AddAsync(expectedCategory);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAsync(expectedCategory.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedCategory);
        }

        [Fact]
        public async Task GetAsync_WithNonExistingCategoryId_ShouldReturnNull()
        {
            // Arrange
            (var context, var repository) = GetContextAndUserRepository();
            var nonExistingId = 1;

            // Act
            var result = await repository.GetAsync(nonExistingId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAsync_WithIsActiveFilter_ShouldReturnOnlyActiveCategories()
        {
            // Arrange
            (var context, var repository) = GetContextAndUserRepository();
            var activeCategory = new Category { Id = 1, Title = "Active Category", IsActive = true };
            var inactiveCategory = new Category { Id = 2, Title = "Inactive Category", IsActive = false };
            await context.Categories.AddRangeAsync(activeCategory, inactiveCategory);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAsync(true, 1, 10);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(1);
            result.Items.Should().ContainEquivalentOf(activeCategory);
        }

        [Fact]
        public async Task CreateAsync_WithValidCategory_ShouldCreateCategory()
        {
            // Arrange
            (var context, var repository) = GetContextAndUserRepository();
            var categoryToCreate = new Category { Title = "New Category", IsActive = true };

            // Act
            var result = await repository.CreateAsync(categoryToCreate);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBe(0);
            context.Categories.Should().ContainEquivalentOf(categoryToCreate, options => options.Excluding(x => x.Id));
        }

        [Fact]
        public async Task UpdateAsync_WithExistingCategoryId_ShouldUpdateCategory()
        {
            // Arrange
            (var context, var repository) = GetContextAndUserRepository();
            var categoryToUpdate = new Category { Id = 1, Title = "Old Category Title", IsActive = true };
            await context.Categories.AddAsync(categoryToUpdate);
            await context.SaveChangesAsync();

            categoryToUpdate.Title = "New Category Title";
            categoryToUpdate.IsActive = false;

            // Act
            await repository.UpdateAsync(categoryToUpdate);

            // Assert
            var result = await repository.GetAsync(categoryToUpdate.Id);
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(categoryToUpdate);
        }

        [Fact]
        public async Task IsUniqueTitleAsync_WithUniqueTitle_ShouldReturnTrue()
        {
            // Arrange
            (var context, var repository) = GetContextAndUserRepository();
            var uniqueTitle = "Unique Title";
            await context.Categories.AddAsync(new Category { Title = "Existing Category" });
            await context.SaveChangesAsync();

            // Act
            var result = await repository.IsUniqueTitleAsync(uniqueTitle);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task GetAsync_ShouldReturnCategory_WhenCategoryExists()
        {
            // Arrange
            (var context, var repository) = GetContextAndUserRepository();
            var category = new Category { Id = 1, Title = "Test Category", IsActive = true };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(category.Id);
            result.Title.Should().Be(category.Title);
            result.IsActive.Should().Be(category.IsActive);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnNull_WhenCategoryDoesNotExist()
        {
            // Act
            (var context, var repository) = GetContextAndUserRepository();
            var result = await repository.GetAsync(1);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Get_ShouldReturnCategory_WhenCategoryExists()
        {
            // Arrange
            (var context, var repository) = GetContextAndUserRepository();
            var category = new Category { Id = 1, Title = "Test Category", IsActive = true };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            // Act
            var result = repository.Get(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(category.Id);
            result.Title.Should().Be(category.Title);
            result.IsActive.Should().Be(category.IsActive);
        }

        [Fact]
        public void Get_ShouldReturnNull_WhenCategoryDoesNotExist()
        {
            // Act
            (var context, var repository) = GetContextAndUserRepository();
            var result = repository.Get(1);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAsync_ShouldReturnPaginatedResult_WhenCategoriesExist()
        {
            // Arrange
            (var context, var repository) = GetContextAndUserRepository();
            context.Categories.AddRange(new List<Category>
        {
            new Category { Id = 1, Title = "Test Category 1", IsActive = true },
            new Category { Id = 2, Title = "Test Category 2", IsActive = false },
            new Category { Id = 3, Title = "Test Category 3", IsActive = true }
        });
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAsync(null, 1, 2);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.Items[0].Id.Should().Be(3);
            result.Items[1].Id.Should().Be(2);
            result.Pagination.PageSize.Should().Be(2);
            result.Pagination.TotalPages.Should().Be(2);
        }

        [Fact]
        public async Task IsUniqueTitleAsync_ReturnsTrue_WhenTitleIsUnique()
        {
            // Arrange
            (var context, var repository) = GetContextAndUserRepository();
            context.Categories.Add(new Category { Id = 1, Title = "Test Category 1", IsActive = true });
            context.Categories.Add(new Category { Id = 2, Title = "Test Category 2", IsActive = false });
            context.SaveChanges();

            // Act
            var result = await repository.IsUniqueTitleAsync("New Category Title", 0);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsUniqueTitleAsync_ReturnsFalse_WhenTitleIsNotUnique()
        {
            // Arrange
            (var context, var repository) = GetContextAndUserRepository();
            context.Categories.Add(new Category { Id = 1, Title = "Test Category 1", IsActive = true });
            context.Categories.Add(new Category { Id = 2, Title = "Test Category 2", IsActive = false });
            context.SaveChanges();

            // Act
            var result = await repository.IsUniqueTitleAsync("Test Category 1", 0);

            // Assert
            Assert.False(result);
        }

        private (CDNDbContext dbContext, ICategoryRepository categoryRepository) GetContextAndUserRepository()
        {
            var options = new DbContextOptionsBuilder<CDNDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new CDNDbContext(options);
            var repository = new CategoryRepository(context);
            return (context, repository);
        }
    }
}
