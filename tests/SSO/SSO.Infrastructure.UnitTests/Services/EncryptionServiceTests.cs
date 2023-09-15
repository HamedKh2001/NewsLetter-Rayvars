using FluentAssertions;
using SSO.Infrastructure.Services;
using Xunit;

namespace SSO.Infrastructure.UnitTests.Services
{
    public class EncryptionServiceTests
    {
        [Fact]
        public void HashPassword_ShouldReturnHashedValue()
        {
            // Arrange
            var encryptionService = new EncryptionService();
            var password = "password123";

            // Act
            var hashedPassword = encryptionService.HashPassword(password);

            // Assert
            hashedPassword.Should().NotBeNull();
            hashedPassword.Should().NotBeEmpty();
            hashedPassword.Should().MatchRegex("^[a-f0-9]{32}$"); // MD5 produces a 32-character hexadecimal string
        }
    }
}
