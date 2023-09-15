using SSO.Application.Contracts.Infrastructure;
using SSO.Application.Features.AuthenticationFeature.Commands.UpdateUserGroup;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SSO.Application.UnitTests.Features.AuthenticationFeature.Commands
{
    public class UpdateUserGroupCommandHandlerTests
    {
        private readonly Mock<IAuthenticationService> _authenticationServiceMock;
        private readonly UpdateUserGroupCommandHandler _handler;

        public UpdateUserGroupCommandHandlerTests()
        {
            _authenticationServiceMock = new Mock<IAuthenticationService>();
            _handler = new UpdateUserGroupCommandHandler(_authenticationServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCallUpdateUserGroupAsync()
        {
            // Arrange
            var request = new UpdateUserGroupCommand();

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _authenticationServiceMock.Verify(x => x.UpdateUserGroupAsync(request, CancellationToken.None), Times.Once);
        }
    }

}
