using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using SharedKernel.Common;
using SSO.Infrastructure.Services;
using System.Security.Claims;
using Xunit;

namespace BRTechGroup.SSO.Infrastructure.Services
{
    public class UserContextServiceTests
    {
        [Fact]
        public void CurrentUser_ReturnsCurrentUser()
        {
            // Arrange
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            var userContextService = new UserContextService(httpContextAccessor.Object);

            var user = new UserIdentitySharedModel
            {
                Id = 1,
                UserName = "testuser",
                FirstName = "Test",
                LastName = "User"
            };

            httpContextAccessor.Setup(x => x.HttpContext.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName)
        })));

            // Act
            var currentUser = userContextService.CurrentUser;

            // Assert
            currentUser.Should().BeEquivalentTo(user);
        }

        [Fact]
        public void CurrenConnection_ReturnsConnectionInfo()
        {
            // Arrange
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            var connection = new Mock<ConnectionInfo>();
            httpContextAccessor.Setup(x => x.HttpContext.Connection).Returns(connection.Object);
            var userContextService = new UserContextService(httpContextAccessor.Object);

            // Act
            var currentConnection = userContextService.CurrenConnection;

            // Assert
            currentConnection.Should().Be(connection.Object);
        }

        [Fact]
        public void UserAgent_ReturnsUserAgent()
        {
            // Arrange
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3";
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var userContextService = new UserContextService(httpContextAccessor.Object);

            // Act
            var userAgent = userContextService.UserAgent;

            // Assert
            userAgent.Should().Be("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");
        }
    }
}
