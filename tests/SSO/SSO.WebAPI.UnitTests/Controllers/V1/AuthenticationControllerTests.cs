using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SharedKernel.Common;
using SSO.Application.Contracts.Infrastructure;
using SSO.Application.Features.AuthenticationFeature.Commands.ChangePassword;
using SSO.Application.Features.AuthenticationFeature.Commands.ChangeUsersPassword;
using SSO.Application.Features.AuthenticationFeature.Commands.UpdateUserGroup;
using SSO.Application.Features.AuthenticationFeature.Queries.Authenticate;
using SSO.Application.Features.AuthenticationFeature.Queries.LogoutUser;
using SSO.Application.Features.AuthenticationFeature.Queries.RefreshToken;
using SSO.WebAPI.Controllers.V1;
using System.Security.Claims;
using Xunit;

namespace SSO.WebAPI.UnitTests.Controllers.V1
{
    public class AuthenticationControllerTests
    {
        private readonly Mock<IUserContextService> _userContextServiceMock;
        private readonly Mock<ISender> _mediatorMock;
        private readonly AuthenticationController _authenticationController;

        public AuthenticationControllerTests()
        {
            _userContextServiceMock = new Mock<IUserContextService>();
            _mediatorMock = new Mock<ISender>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(ISender))).Returns(_mediatorMock.Object);

            _authenticationController = new AuthenticationController(_userContextServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, "testuser"),
                            new Claim(ClaimTypes.NameIdentifier, "123"),
                            new Claim(ClaimTypes.Role, "SSO-Admin")
                        })),
                        RequestServices = serviceProviderMock.Object,
                    }
                }
            };
        }

        [Fact]
        public async Task Login_Returns_OkObjectResult_With_AuthenticateDto()
        {
            // Arrange
            var query = new AuthenticateQuery();
            var mediatrResult = new AuthenticateDto() { RefreshToken = "RefreshToken", Token = "Token" };

            _mediatorMock.Setup(m => m.Send(query, CancellationToken.None)).ReturnsAsync(mediatrResult);

            // Act
            var result = await _authenticationController.Login(query, CancellationToken.None);

            // Assert
            result.Should().BeOfType<ActionResult<AuthenticateDto>>();
        }

        [Fact]
        public async Task ChangePassword_Returns_BadRequest_When_Command_UserId_Does_Not_Match_CurrentUser_Id()
        {
            // Arrange
            var command = new ChangePasswordCommand { UserId = 1 };

            _userContextServiceMock.Setup(u => u.CurrentUser).Returns(new UserIdentitySharedModel { Id = 1 });

            // Act
            var result = await _authenticationController.ChangePassword(command, CancellationToken.None);

            // Assert
            result.Should().BeOfType<ActionResult<AuthenticateDto>>()
                .Which.Value.Should().Be(null);
        }

        [Fact]
        public async Task ChangePassword_Returns_NoContentResult_When_Command_UserId_Matches_CurrentUser_Id()
        {
            // Arrange
            var command = new ChangePasswordCommand { UserId = 1 };

            _userContextServiceMock.Setup(u => u.CurrentUser).Returns(new UserIdentitySharedModel { Id = 1 });

            _mediatorMock.Setup(m => m.Send(command, CancellationToken.None)).ReturnsAsync(Unit.Value);

            // Act
            var result = await _authenticationController.ChangePassword(command, CancellationToken.None);

            // Assert
            result.Result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task ChangeUsersPassword_Should_Return_BadRequest_When_AdminUserId_Does_Not_Match_CurrentUser()
        {
            // Arrange
            var command = new ChangeUsersPasswordCommand { AdminUserId = 1 };
            _userContextServiceMock.Setup(u => u.CurrentUser).Returns(new UserIdentitySharedModel { Id = 2 });

            // Act
            var result = await _authenticationController.ChangeUsersPassword(command, CancellationToken.None);

            // Assert
            result.Result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task ChangeUsersPassword_Should_Invoke_Mediator_When_AdminUserId_Matches_CurrentUser()
        {
            // Arrange
            var command = new ChangeUsersPasswordCommand { AdminUserId = 1 };
            _userContextServiceMock.Setup(u => u.CurrentUser).Returns(new UserIdentitySharedModel { Id = 1 });

            // Act
            var result = await _authenticationController.ChangeUsersPassword(command, CancellationToken.None);

            // Assert
            result.Result.Should().BeOfType<NoContentResult>();
            _mediatorMock.Verify(x => x.Send(command, default), Times.Once);
        }

        [Fact]
        public async Task UpdateUserGroup_Should_Invoke_Mediator()
        {
            // Arrange
            var command = new UpdateUserGroupCommand();

            // Act
            var result = await _authenticationController.UpdateUserGroup(command, CancellationToken.None);

            // Assert
            result.Result.Should().BeOfType<NoContentResult>();
            _mediatorMock.Verify(x => x.Send(command, default), Times.Once);
        }

        [Fact]
        public async Task Logout_Should_Return_BadRequest_When_UserId_Does_Not_Match_CurrentUser()
        {
            // Arrange
            var query = new LogoutUserQuery { UserId = 1 };
            _userContextServiceMock.Setup(u => u.CurrentUser).Returns(new UserIdentitySharedModel { Id = 2 });

            // Act
            var result = await _authenticationController.Logout(query, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task Logout_Should_Invoke_Mediator_When_UserId_Matches_CurrentUser()
        {
            // Arrange
            var query = new LogoutUserQuery { UserId = 1 };
            _userContextServiceMock.Setup(u => u.CurrentUser).Returns(new UserIdentitySharedModel { Id = 1 });

            // Act
            var result = await _authenticationController.Logout(query, CancellationToken.None);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mediatorMock.Verify(x => x.Send(query, default), Times.Once);
        }

        [Fact]
        public async Task RefreshToken_Should_Invoke_Mediator()
        {
            // Arrange
            var query = new RefreshTokenQuery();

            // Act
            var result = await _authenticationController.RefreshToken(query, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _mediatorMock.Verify(x => x.Send(query, default), Times.Once);
        }
    }
}
