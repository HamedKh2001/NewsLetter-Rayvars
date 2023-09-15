using SSO.Application.Contracts.Infrastructure;
using SSO.Application.Features.AuthenticationFeature.Queries.Authenticate;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SSO.Application.UnitTests.Features.AuthenticationFeature.Queries
{
    public class AuthenticateQueryHandlerTests
    {
        private readonly Mock<IAuthenticationService> _authenticationServiceMock;
        private readonly AuthenticateQueryHandler _handler;

        public AuthenticateQueryHandlerTests()
        {
            _authenticationServiceMock = new Mock<IAuthenticationService>();
            _handler = new AuthenticateQueryHandler(_authenticationServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCallAuthenticateAsync()
        {
            // Arrange
            var request = new AuthenticateQuery();

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _authenticationServiceMock.Verify(x => x.AuthenticateAsync(request, CancellationToken.None), Times.Once);
        }
    }
}
