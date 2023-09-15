using Moq;
using SSO.Application.Contracts.Infrastructure;
using SSO.Application.Features.AuthenticationFeature.Queries.Authenticate;
using SSO.Application.Features.AuthenticationFeature.Queries.RefreshToken;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SSO.Application.UnitTests.Features.AuthenticationFeature.Queries
{
    public class RefreshTokenQueryHandlerTests
    {
        private readonly Mock<IAuthenticationService> _authenticationServiceMock = new Mock<IAuthenticationService>();
        private readonly RefreshTokenQueryHandler _handler;

        public RefreshTokenQueryHandlerTests()
        {
            _handler = new RefreshTokenQueryHandler(_authenticationServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsAuthenticateDto()
        {
            // Arrange
            var query = new RefreshTokenQuery();
            var expectedDto = new AuthenticateDto();
            _authenticationServiceMock.Setup(x => x.RefreshTokenAsync(query, CancellationToken.None)).ReturnsAsync(expectedDto);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(expectedDto, result);
        }
    }

}
