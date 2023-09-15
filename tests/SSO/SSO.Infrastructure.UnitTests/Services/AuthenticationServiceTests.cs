using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using SharedKernel.Contracts.Infrastructure;
using SharedKernel.Exceptions;
using SSO.Application.Common.Settings;
using SSO.Application.Contracts.Infrastructure;
using SSO.Application.Contracts.Persistence;
using SSO.Application.Features.AuthenticationFeature.Commands.ChangePassword;
using SSO.Application.Features.AuthenticationFeature.Commands.UpdateUserGroup;
using SSO.Application.Features.AuthenticationFeature.Queries.Authenticate;
using SSO.Application.Features.AuthenticationFeature.Queries.LogoutUser;
using SSO.Domain.Entities;
using SSO.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SSO.Infrastructure.UnitTests.Services
{
    public class AuthenticationServiceTests
    {
        private readonly AuthenticationService _authenticationService;
        private readonly Mock<IEncryptionService> _encryptionServiceMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUserLoginRepository> _userLoginRepositoryMock;
        private readonly Mock<IGroupRepository> _groupRepositoryMock;
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
        private readonly Mock<IDateTimeService> _dateTimeServiceMock;
        private readonly Mock<IOptionsMonitor<BearerTokensConfigurationModel>> _bearerTokensMock;
        private readonly Mock<IUserContextService> _userContextServiceMock;
        private readonly Mock<ITokenCacheService> _tokenCacheServiceMock;

        public AuthenticationServiceTests()
        {
            _encryptionServiceMock = new Mock<IEncryptionService>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _userLoginRepositoryMock = new Mock<IUserLoginRepository>();
            _groupRepositoryMock = new Mock<IGroupRepository>();
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            _dateTimeServiceMock = new Mock<IDateTimeService>();
            _bearerTokensMock = new Mock<IOptionsMonitor<BearerTokensConfigurationModel>>();
            _userContextServiceMock = new Mock<IUserContextService>();
            _tokenCacheServiceMock = new Mock<ITokenCacheService>();

            _authenticationService = new AuthenticationService(
                _encryptionServiceMock.Object,
                _userRepositoryMock.Object,
                _userLoginRepositoryMock.Object,
                _groupRepositoryMock.Object,
                _refreshTokenRepositoryMock.Object,
                _dateTimeServiceMock.Object,
                _bearerTokensMock.Object,
                _userContextServiceMock.Object,
                _tokenCacheServiceMock.Object
            );
        }

        [Fact]
        public async Task AuthenticateAsync_WhenUserNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var request = new AuthenticateQuery { UserName = "testuser", Password = "testpassword" };
            _userRepositoryMock.Setup(x => x.GetUserWithRolesAsync(request.UserName, It.IsAny<string>(), CancellationToken.None)).ReturnsAsync((User)null);

            // Act
            Func<Task> action = async () => await _authenticationService.AuthenticateAsync(request, CancellationToken.None);

            // Assert
            await action.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            var request = new ChangePasswordCommand { UserId = 1, CurrentPassword = "password1", NewPassword = "password2" };
            _userRepositoryMock.Setup(x => x.GetUserByPasswordAsync(request.UserId, It.IsAny<string>(), CancellationToken.None)).ReturnsAsync((User)null);

            // Act
            Func<Task> act = async () => await _authenticationService.ChangePasswordAsync(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>().WithMessage($"*{nameof(User)}*{request.UserId}*");
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldUpdateUserPassword_WhenUserExists()
        {
            // Arrange
            var request = new ChangePasswordCommand { UserId = 1, CurrentPassword = "password1", NewPassword = "password2" };
            var user = new User { Id = request.UserId, Password = "password1" };
            _userRepositoryMock.Setup(x => x.GetUserByPasswordAsync(request.UserId, It.IsAny<string>(), CancellationToken.None)).ReturnsAsync(user);
            _encryptionServiceMock.Setup(x => x.HashPassword(request.NewPassword)).Returns("hashed_password");

            // Act
            await _authenticationService.ChangePasswordAsync(request, CancellationToken.None);

            // Assert
            user.Password.Should().Be("hashed_password");
            _userRepositoryMock.Verify(x => x.UpdateAsync(user, CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task UpdateUserGroupAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            var request = new UpdateUserGroupCommand { UserId = 1, GroupIds = new List<int> { 1, 2 } };
            _userRepositoryMock.Setup(x => x.GetUserWithRolesAsync(request.UserId, CancellationToken.None)).ReturnsAsync((User)null);

            // Act
            Func<Task> act = async () => await _authenticationService.UpdateUserGroupAsync(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>().WithMessage($"*{nameof(User)}*{request.UserId}*");
        }

        [Fact]
        public async Task LogoutAsync_ThrowsBadRequestException_WhenUserDoesNotExist()
        {
            // Arrange
            var bearerToken = new BearerTokensConfigurationModel()
            {
                Key = "BRTechSystemRandomKeyToken",
                Issuer = "http://localhost:7000/"
            };

            var request = new LogoutUserQuery
            {
                UserId = 1,
                AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIxMmJjODE2Zi0zZmM3LTQyNGYtYjMzZC05NzFlMmUwMDdjNzciLCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjcwMDAvIiwiaWF0IjoxNjc4NjI1MTIxLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zdXJuYW1lIjoi2LPbjNiz2KrZhSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2dpdmVubmFtZSI6ItmF2K_bjNixIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvdXJpIjoiIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjpbIkF1dGhlbnRpY2F0aW9uLVVwZGF0ZVVzZXJHcm91cCIsIkF1dGhlbnRpY2F0aW9uLUNoYW5nZVVzZXJzUGFzc3dvcmQiLCJVc2VyLUdldCIsIlVzZXItR2V0QnlJZCIsIlVzZXItR2V0QnlVc2VyTmFtZSIsIlVzZXItR2V0VXNlclJvbGVzIiwiVXNlci1HZXRVc2VyR3JvdXBzIiwiVXNlci1DcmVhdGUiLCJVc2VyLVVwZGF0ZSIsIlVzZXItRGVsZXRlIiwiVXNlci1VcGxvYWRJbWFnZSIsIlJvbGUtR2V0IiwiUm9sZS1VcGRhdGUiLCJSb2xlLUdldFJvbGVHcm91cHMiLCJSb2xlLUdldFJvbGVVc2VycyIsIkdyb3VwLUdldCIsIkdyb3VwLUdldEJ5SWQiLCJHcm91cC1HZXRBbGwiLCJHcm91cC1HZXRHcm91cFVzZXJzIiwiR3JvdXAtR2V0R3JvdXBSb2xlcyIsIkdyb3VwLUNyZWF0ZSIsIkdyb3VwLVVwZGF0ZSIsIkdyb3VwLVVwZGF0ZUdyb3VwUm9sZSIsIkdyb3VwLVVwZGF0ZUdyb3VwVXNlcnMiLCJHcm91cC1EZWxldGUiLCJFbXBsb3llZS1HZXQiLCJFbXBsb3llZS1HZXRCeUlkIiwiRW1wbG95ZWUtR2V0QnlVc2VySWQiLCJFbXBsb3llZS1DcmVhdGUiLCJFbXBsb3llZS1VcGRhdGUiLCJFbXBsb3llZS1EZWxldGUiLCJFbXBsb3llZURyaXZlci1HZXQiLCJFbXBsb3llZURyaXZlci1HZXRCeUlkIiwiRW1wbG95ZWVEcml2ZXItQ3JlYXRlIiwiRW1wbG95ZWVEcml2ZXItVXBkYXRlIiwiRW1wbG95ZWVEcml2ZXItRGVsZXRlIiwiRW1wbG95ZWVFbXBsb3ltZW50LUdldCIsIkVtcGxveWVlRW1wbG95bWVudC1HZXRCeUlkIiwiRW1wbG95ZWVFbXBsb3ltZW50LUNyZWF0ZSIsIkVtcGxveWVlRW1wbG95bWVudC1VcGRhdGUiLCJFbXBsb3llZUVtcGxveW1lbnQtRGVsZXRlIiwiRW1wbG95ZWVOZXQtR2V0IiwiRW1wbG95ZWVOZXQtR2V0QnlJZCIsIkVtcGxveWVlTmV0LUNyZWF0ZSIsIkVtcGxveWVlTmV0LVVwZGF0ZSIsIkVtcGxveWVlTmV0LURlbGV0ZSIsIk9yZ2FuaXphdGlvbi1HZXQiLCJPcmdhbml6YXRpb24tR2V0QnlJZCIsIk9yZ2FuaXphdGlvbi1HZXRBbGwiLCJPcmdhbml6YXRpb24tQ3JlYXRlIiwiT3JnYW5pemF0aW9uLVVwZGF0ZSIsIk9yZ2FuaXphdGlvbi1EZWxldGUiLCJKb2JUeXBlLUdldCIsIkpvYlR5cGUtQ3JlYXRlIiwiSm9iVHlwZS1VcGRhdGUiLCJKb2JUeXBlLURlbGV0ZSIsIkpvYi1HZXQiLCJKb2ItR2V0QnlJZCIsIkpvYi1DcmVhdGUiLCJKb2ItVXBkYXRlIiwiSm9iLURlbGV0ZSIsIlBvc2l0aW9uLUdldCIsIlBvc2l0aW9uLUdldEJ5SWQiLCJQb3NpdGlvbi1DcmVhdGUiLCJQb3NpdGlvbi1VcGRhdGUiLCJQb3NpdGlvbi1EZWxldGUiLCJQcm9qZWN0LUdldCIsIlByb2plY3QtR2V0QnlJZCIsIlByb2plY3QtR2V0QnlDb2RlIiwiUHJvamVjdC1DcmVhdGUiLCJQcm9qZWN0LVVwZGF0ZSIsIlByb2plY3QtRGVsZXRlIiwiUHJvamVjdFRlYW0tR2V0IiwiUHJvamVjdFRlYW0tR2V0QnlJZCIsIlByb2plY3RUZWFtLUNyZWF0ZSIsIlByb2plY3RUZWFtLVVwZGF0ZSIsIlByb2plY3RUZWFtLURlYWN0aXZlIiwiVGVhbVJvbGUtR2V0IiwiVGVhbVJvbGUtR2V0QnlJZCIsIlRlYW1Sb2xlLUNyZWF0ZSIsIlRlYW1Sb2xlLVVwZGF0ZSIsIlRlYW1Sb2xlLURlbGV0ZSIsIkhvbGlkYXktR2V0IiwiSG9saWRheS1HZXRCeUlkIiwiSG9saWRheS1DcmVhdGUiLCJIb2xpZGF5LVVwZGF0ZSIsIkhvbGlkYXktRGVsZXRlIiwiT3ZlcnRpbWVSdWxlLUdldCIsIk92ZXJ0aW1lUnVsZS1HZXRCeUlkIiwiT3ZlcnRpbWVSdWxlLUNyZWF0ZSIsIk92ZXJ0aW1lUnVsZS1VcGRhdGUiLCJPdmVydGltZVJ1bGUtRGVsZXRlIiwiT3ZlcnRpbWVSdWxlU3BlYy1HZXQiLCJPdmVydGltZVJ1bGVTcGVjLUdldEJ5SWQiLCJPdmVydGltZVJ1bGVTcGVjLUNyZWF0ZSIsIk92ZXJ0aW1lUnVsZVNwZWMtVXBkYXRlIiwiT3ZlcnRpbWVSdWxlU3BlYy1EZWxldGUiLCJXb3JrQ2FsZW5kYXItR2V0IiwiV29ya0NhbGVuZGFyLUdldEJ5SWQiLCJXb3JrQ2FsZW5kYXItQ3JlYXRlIiwiV29ya0NhbGVuZGFyLVVwZGF0ZSIsIldvcmtDYWxlbmRhci1EZWxldGUiLCJXb3JrQ2FsZW5kYXJTcGVjLUdldCIsIldvcmtDYWxlbmRhclNwZWMtR2V0QnlJZCIsIldvcmtDYWxlbmRhclNwZWMtQ3JlYXRlIiwiV29ya0NhbGVuZGFyU3BlYy1VcGRhdGUiLCJXb3JrQ2FsZW5kYXJTcGVjLURlbGV0ZSIsIkxlYXZlLUdldCIsIkxlYXZlLUdldEJ5SWQiLCJMZWF2ZS1DcmVhdGUiLCJMZWF2ZS1VcGRhdGUiLCJMZWF2ZS1EZWxldGUiLCJJbkNvbXBsZXRlUHJlc2VuY2UtR2V0IiwiSW5Db21wbGV0ZVByZXNlbmNlLUdldEJ5SWQiLCJJbkNvbXBsZXRlUHJlc2VuY2UtQ3JlYXRlIiwiSW5Db21wbGV0ZVByZXNlbmNlLVVwZGF0ZSIsIkluQ29tcGxldGVQcmVzZW5jZS1EZWxldGUiLCJPbkNhbGwtR2V0IiwiT25DYWxsLUdldEJ5SWQiLCJPbkNhbGwtQ3JlYXRlIiwiT25DYWxsLVVwZGF0ZSIsIk9uQ2FsbC1EZWxldGUiLCJXb3JrT3V0U2lkZS1HZXQiLCJXb3JrT3V0U2lkZS1HZXRCeUlkIiwiV29ya091dFNpZGUtQ3JlYXRlIiwiV29ya091dFNpZGUtVXBkYXRlIiwiV29ya091dFNpZGUtRGVsZXRlIiwiRHJpdmVyRGlzdGFuY2UtR2V0IiwiRHJpdmVyRGlzdGFuY2UtR2V0QnlJZCIsIkRyaXZlckRpc3RhbmNlLUNyZWF0ZSIsIkRyaXZlckRpc3RhbmNlLVVwZGF0ZSIsIkRyaXZlckRpc3RhbmNlLURlbGV0ZSIsIk1pc3Npb24tR2V0IiwiTWlzc2lvbi1HZXRCeUlkIiwiTWlzc2lvbi1DcmVhdGUiLCJNaXNzaW9uLVVwZGF0ZSIsIk1pc3Npb24tRGVsZXRlIiwiTWlzc2lvbkRldGFpbC1HZXQiLCJNaXNzaW9uRGV0YWlsLUdldEJ5SWQiLCJNaXNzaW9uRGV0YWlsLUNyZWF0ZSIsIk1pc3Npb25EZXRhaWwtVXBkYXRlIiwiTWlzc2lvbkRldGFpbC1EZWxldGUiLCJEYWlseVRpbWVTaGVldC1HZXQiLCJEYWlseVRpbWVTaGVldC1HZXRCeUlkIiwiSW5jZW50aXZlT3ZlclRpbWUtR2V0IiwiSW5jZW50aXZlT3ZlclRpbWUtR2V0QnlJZCIsIkluY2VudGl2ZU92ZXJUaW1lLUNyZWF0ZSIsIkluY2VudGl2ZU92ZXJUaW1lLVVwZGF0ZSIsIkluY2VudGl2ZU92ZXJUaW1lLURlbGV0ZSIsIkRhaWx5UmVwb3J0LUdldCIsIkRhaWx5UmVwb3J0LUdldEJ5SWQiLCJEYWlseVJlcG9ydC1DcmVhdGUiLCJEYWlseVJlcG9ydC1VcGRhdGUiLCJEYWlseVJlcG9ydC1EZWxldGUiLCJQcm9qZWN0UmVwb3J0LURhaWx5UHJvamVjdFJlcG9ydCIsIkNhdGVnb3J5LUdldCIsIkNhdGVnb3J5LUdldEJ5SWQiLCJDYXRlZ29yeS1DcmVhdGUiLCJDYXRlZ29yeS1VcGRhdGUiLCJMb2dSZXBvcnQtR2V0IiwiRW1wbG95ZWUtTXlFbXBsb3llZXMiLCJFbXBsb3llZS1NeUVtcGxveWVlIiwiQ2FydGFibGVDb25maWctR2V0IiwiQ2FydGFibGVDb25maWctR2V0QnlJZCIsIkNhcnRhYmxlQ29uZmlnLUNyZWF0ZSIsIkNhcnRhYmxlQ29uZmlnLVVwZGF0ZSIsIkNhcnRhYmxlQ29uZmlnLURlbGV0ZSIsIkNhcnRhYmxlQ29uZmlnUGVybWlzc2lvbi1HZXQiLCJDYXJ0YWJsZUNvbmZpZ1Blcm1pc3Npb24tR2V0QnlJZCIsIkNhcnRhYmxlQ29uZmlnUGVybWlzc2lvbi1DcmVhdGUiLCJDYXJ0YWJsZUNvbmZpZ1Blcm1pc3Npb24tVXBkYXRlIiwiQ2FydGFibGVDb25maWdQZXJtaXNzaW9uLURlbGV0ZSIsIlJlcXVlc3QtR2V0IiwiUmVxdWVzdC1HZXRCeUlkIiwiUmVxdWVzdC1DcmVhdGVJbmNlbnRpdmVPdmVyVGltZSIsIlJlcXVlc3RUeXBlLUdldCIsIlJlcXVlc3RUeXBlLUdldEJ5SWQiLCJSZXF1ZXN0VHlwZS1DcmVhdGUiLCJSZXF1ZXN0VHlwZS1VcGRhdGUiLCJSZXF1ZXN0VHlwZS1EZWxldGUiLCJIdWItTm90aWZpY2F0aW9uIiwiUmVxdWVzdC1DcmVhdGVJbkNvbXBsZXRlUHJlc2VuY2UiLCJSZXF1ZXN0LUNyZWF0ZUxlYXZlIiwiUmVxdWVzdC1DcmVhdGVPbkNhbGwiLCJSZXF1ZXN0LUNyZWF0ZVdvcmtPdXRTaWRlIiwiUmVxdWVzdC1DcmVhdGVNaXNzaW9uIiwiUmVxdWVzdC1DcmVhdGVNaXNzaW9uRGV0YWlsIiwiUmVxdWVzdC1VcGRhdGVXb3JrRmxvdyIsIlJlcXVlc3QtR2V0SW5jZW50aXZlT3ZlclRpbWUiLCJSZXF1ZXN0LUdldEluQ29tcGxldGVQcmVzZW5jZSIsIlJlcXVlc3QtR2V0TGVhdmUiLCJSZXF1ZXN0LUdldGVPbkNhbGwiLCJSZXF1ZXN0LUdldFdvcmtPdXRTaWRlIiwiUmVxdWVzdC1HZXRNaXNzaW9uIiwiUmVxdWVzdC1HZXRNaXNzaW9uRGV0YWlsIiwiV29ya0Zsb3ctVXBkYXRlIiwiV29ya0Zsb3ctR2V0IiwiV29ya0Zsb3ctR2V0QnlJZCIsIlRpbWVTaGVldC1HZXQiLCJUaW1lU2hlZXQtR2V0TXlUaW1lU2hlZXQiLCJUaW1lU2hlZXQtR2V0TXlUaW1lU2hlZXRzIiwiRW1wbG95ZWVUaW1lU2hlZXROb3JtYWwtR2V0IiwiRW1wbG95ZWVUaW1lU2hlZXROb3JtYWwtR2V0QnlJZCIsIkVtcGxveWVlVGltZVNoZWV0Tm9ybWFsLUNyZWF0ZSIsIkVtcGxveWVlVGltZVNoZWV0Tm9ybWFsLVVwZGF0ZSIsIkRhaWx5VGltZVNoZWV0RmlsbE5vcm1hbFBlcm1pc3Npb24tR2V0IiwiRGFpbHlUaW1lU2hlZXRGaWxsTm9ybWFsUGVybWlzc2lvbi1HZXRCeUlkIiwiRGFpbHlUaW1lU2hlZXRGaWxsTm9ybWFsUGVybWlzc2lvbi1VcGRhdGUiLCJEYWlseVRpbWVTaGVldEZpbGxOb3JtYWxQZXJtaXNzaW9uLURlbGV0ZSIsIkRhaWx5VGltZVNoZWV0RmlsbE5vcm1hbC1HZXRCeUVtcGxveWVlSWQiLCJEYWlseVRpbWVTaGVldEZpbGxOb3JtYWwtR2V0QnlNb250aGx5UGVyaW9kIiwiRGFpbHlUaW1lU2hlZXRGaWxsTm9ybWFsLUFkZE9yVXBkYXRlIiwiUGVybWlzc2lvbnMuTG9va3VwLlZpZXciLCJQZXJtaXNzaW9ucy5Mb29rdXAuQ3JlYXRlIiwiUGVybWlzc2lvbnMuTG9va3VwLkVkaXQiLCJQZXJtaXNzaW9ucy5Mb29rdXAuRGVsZXRlIiwiUGVybWlzc2lvbnMuTG9va3VwLkV4cG9ydCIsIlBlcm1pc3Npb25zLkxvb2t1cC5WaWV3TWVudSIsIlBlcm1pc3Npb25zLlBlcnNvbi5WaWV3IiwiUGVybWlzc2lvbnMuUGVyc29uLkNyZWF0ZSIsIlBlcm1pc3Npb25zLlBlcnNvbi5FZGl0IiwiUGVybWlzc2lvbnMuUGVyc29uLkRlbGV0ZSIsIlBlcm1pc3Npb25zLlBlcnNvbi5FeHBvcnQiLCJQZXJtaXNzaW9ucy5QZXJzb24uRGVncmVlSGlzdG9yeSIsIlBlcm1pc3Npb25zLlBlcnNvbi5WaWV3TWVudSIsIlBlcm1pc3Npb25zLlN0dWRlbnRzLlZpZXciLCJQZXJtaXNzaW9ucy5TdHVkZW50cy5DcmVhdGUiLCJQZXJtaXNzaW9ucy5TdHVkZW50cy5FZGl0IiwiUGVybWlzc2lvbnMuU3R1ZGVudHMuRGVsZXRlIiwiUGVybWlzc2lvbnMuU3R1ZGVudHMuRXhwb3J0IiwiUGVybWlzc2lvbnMuU3R1ZGVudHMuRGVncmVlSGlzdG9yeSIsIlBlcm1pc3Npb25zLlN0dWRlbnRzLlZpZXdNZW51IiwiUGVybWlzc2lvbnMuUGVyc29uZWxzLlZpZXciLCJQZXJtaXNzaW9ucy5QZXJzb25lbHMuQ3JlYXRlIiwiUGVybWlzc2lvbnMuUGVyc29uZWxzLkVkaXQiLCJQZXJtaXNzaW9ucy5QZXJzb25lbHMuRGVsZXRlIiwiUGVybWlzc2lvbnMuUGVyc29uZWxzLkV4cG9ydCIsIlBlcm1pc3Npb25zLlBlcnNvbmVscy5EZWdyZWVIaXN0b3J5IiwiUGVybWlzc2lvbnMuUGVyc29uZWxzLlZpZXdNZW51IiwiUGVybWlzc2lvbnMuVGVhY2hlcnMuVmlldyIsIlBlcm1pc3Npb25zLlRlYWNoZXJzLkNyZWF0ZSIsIlBlcm1pc3Npb25zLlRlYWNoZXJzLkVkaXQiLCJQZXJtaXNzaW9ucy5UZWFjaGVycy5EZWxldGUiLCJQZXJtaXNzaW9ucy5UZWFjaGVycy5FeHBvcnQiLCJQZXJtaXNzaW9ucy5UZWFjaGVycy5EZWdyZWVIaXN0b3J5IiwiUGVybWlzc2lvbnMuVGVhY2hlcnMuVmlld01lbnUiLCJQZXJtaXNzaW9ucy5QZXJzb25hbEF0dGFjaG1lbnQuVmlld01lbnUiLCJQZXJtaXNzaW9ucy5Db3Vyc2UuVmlld01lbnUiLCJQZXJtaXNzaW9ucy5DZXJ0aWZpY2F0ZS5WaWV3TWVudSIsIlBlcm1pc3Npb25zLlN0YWdlLlZpZXdNZW51IiwiUGVybWlzc2lvbnMuUGh5c2ljYWxQbGFjZS5WaWV3TWVudSIsIlBlcm1pc3Npb25zLkNyZWRpdG9yLlZpZXdNZW51IiwiUGVybWlzc2lvbnMuRGVidG9yLlZpZXdNZW51IiwiUGVybWlzc2lvbnMuRmxpZ2h0UmVxdWVzdFByaWNlLlZpZXdNZW51IiwiUGVybWlzc2lvbnMuTWFuYWdlRXZlbnRzLlZpZXdNZW51IiwiUGVybWlzc2lvbnMuVGVybS5WaWV3TWVudSIsIlBlcm1pc3Npb25zLkxpYnJhcnlFZHVjYXRpb24uVmlld01lbnUiLCJQZXJtaXNzaW9ucy5UZXN0bGlzdC5WaWV3TWVudSIsIlBlcm1pc3Npb25zLlJlZ2lzdGVyUmVxdWVzdHMuVmlld01lbnUiLCJQZXJtaXNzaW9ucy5EYWlseVJvb21DaGFuZ2UuVmlld01lbnUiLCJQZXJtaXNzaW9ucy5NeUNsYXNzZXNsaXN0UHJvZmVzc29yLlZpZXdNZW51IiwiUGVybWlzc2lvbnMuTGlicmFyeVByb2Zlc3Nvci5WaWV3TWVudSIsIlBlcm1pc3Npb25zLlByb2Zlc3NvckNhbGFuZGFyLlZpZXdNZW51IiwiUGVybWlzc2lvbnMuQ291cnNlQ2xhc3NCb29rLkNyZWF0ZSIsIlBlcm1pc3Npb25zLkNvdXJzZUJvb2suQ3JlYXRlIiwiUGVybWlzc2lvbnMuQ2VydGlmaWNhdGVSZXF1ZXN0SXNzdWUuVmlld01lbnUiLCJQZXJtaXNzaW9ucy5NeUNlcnRpZmljYXRlUmVxdWVzdC5WaWV3TWVudSIsIlBlcm1pc3Npb25zLkZsaWdodFJlcXVlc3QuVmlld01lbnUiLCJQZXJtaXNzaW9ucy5UZXJtU2VsZWN0aW9uLlZpZXdNZW51IiwiUGVybWlzc2lvbnMuQ3JlZGl0Q2hhcmdlLlZpZXdNZW51IiwiUGVybWlzc2lvbnMuTGlicmFyeVN0dWRlbnRzLlZpZXdNZW51IiwiUGVybWlzc2lvbnMuUmVzZXJ2YXRpb25TaW5nbGVDb3Vyc2UuVmlld01lbnUiLCJQZXJtaXNzaW9ucy5NeWNsYXNzZXNTdHVkZW50LlZpZXdNZW51IiwiUGVybWlzc2lvbnMuU2luZ2xlQ291cmNlU3R1ZGVudC5WaWV3TWVudSIsIlBlcm1pc3Npb25zLkZsaWdodFJlcXVlc3RsaXN0LlZpZXdNZW51IiwiUGVybWlzc2lvbnMuU3R1ZGVudFNvbG8uVmlld01lbnUiLCJQZXJtaXNzaW9ucy5XZWJzaXRlRmllbGRzLlZpZXdNZW51IiwiUGVybWlzc2lvbnMuV2ViU2l0ZVN1YmplY3QuVmlld01lbnUiLCJQZXJtaXNzaW9ucy5XZWJzaXRlQ29udGFjdEluZm9ybWF0aW9uLlZpZXdNZW51IiwiUGVybWlzc2lvbnMuV2Vic2l0ZVNsaWRlci5WaWV3TWVudSIsIlBlcm1pc3Npb25zLldlYlNpdGVNZW51LlZpZXdNZW51IiwiUGVybWlzc2lvbnMuV2Vic2l0ZVF1aWNrQWNjZXNzLlZpZXdNZW51IiwiUGVybWlzc2lvbnMuV2Vic2l0ZUZhY2lsaXRpZXMuVmlld01lbnUiLCJQZXJtaXNzaW9ucy5XZWJzaXRlQ291cnNlLlZpZXdNZW51IiwiUGVybWlzc2lvbnMuV2Vic2l0ZU5ld3MuVmlld01lbnUiLCJQZXJtaXNzaW9ucy5XZWJzaXRlQ29tcGxldGlvbkluZm8uVmlld01lbnUiLCJQZXJtaXNzaW9ucy5UZXJtc1JlcG9ydC5WaWV3TWVudSIsIlBlcm1pc3Npb25zLlRlYWNoZXJzUmVwb3J0LlZpZXdNZW51IiwiUGVybWlzc2lvbnMuU3R1ZGVudHNSZXBvcnQuVmlld01lbnUiLCJQZXJtaXNzaW9ucy5Db250cmFjdFN0dWRlbnRzUmVwb3J0LlZpZXdNZW51IiwiUGVybWlzc2lvbnMuQ2VudGVyRG9jR2VuZXJhbC5WaWV3TWVudSIsIlBlcm1pc3Npb25zLlJvbGUuVmlld01lbnUiLCJQZXJtaXNzaW9ucy5Vc2Vycy5WaWV3TWVudSIsIlBlcm1pc3Npb25zLk15QWNjb3VudFR1cm5vdmVyUmVwb3J0LlZpZXdNZW51IiwiUGVybWlzc2lvbnMuRmxpZ2h0UmVxdWVzdHNSZXBvcnQuVmlld01lbnUiLCJQZXJtaXNzaW9ucy5SZWdpc3RyYXRpb25SZXF1ZXN0c1JlcG9ydC5WaWV3TWVudSIsIlBlcm1pc3Npb25zLlBlcnNvbkdyb3VwTG9va3VwLlZpZXciLCJQZXJtaXNzaW9ucy5QZXJzb25Hcm91cExvb2t1cC5DcmVhdGUiLCJQZXJtaXNzaW9ucy5QZXJzb25Hcm91cExvb2t1cC5FZGl0IiwiUGVybWlzc2lvbnMuUGVyc29uR3JvdXBMb29rdXAuRGVsZXRlIiwiUGVybWlzc2lvbnMuUGVyc29uR3JvdXBMb29rdXAuRXhwb3J0IiwiUGVybWlzc2lvbnMuUGVyc29uR3JvdXBMb29rdXAuVmlld01lbnUiLCJQZXJtaXNzaW9ucy5UZXJtVjIuVmlld01lbnUiLCJQZXJtaXNzaW9ucy5OZXdUZXJtU2VsZWN0aW9uLlZpZXdNZW51IiwiUGVybWlzc2lvbnMuTXlUZXJtc1N0dWRlbnQuVmlld01lbnUiLCJEYWlseVRpbWVTaGVldEZpbGxOb3JtYWwtRW1wbG95ZWVEYWlseVRpbWVTaGVldE5vcm1hbCIsIkRhaWx5VGltZVNoZWV0RmlsbE5vcm1hbC1EYWlseVRpbWVTaGVldE5vcm1hbENhcnRhYmxlIiwiU1NPLUFkbWluIiwiQ0ROLUFkbWluIiwiTXkiLCJVLUciLCJVLUMiLCJVLVUiLCJVLUQiLCJVLVVJIiwiRS1HIiwiRS1DIiwiRS1VIiwiRS1EIiwiRUQtRyIsIkVELUMiLCJFRC1VIiwiRUQtRCIsIkVFLUciLCJFRS1DIiwiRUUtVSIsIkVFLUQiLCJFTi1HIiwiRU4tQyIsIkVOLVUiLCJFTi1EIiwiTy1HIiwiTy1DIiwiTy1VIiwiTy1EIiwiSlQtRyIsIkpULUMiLCJKVC1VIiwiSlQtRCIsIkotRyIsIkotQyIsIkotVSIsIkotRCIsIlAtRyIsIlAtQyIsIlAtVSIsIlAtRCIsIlROLUciLCJUTi1DIiwiVE4tVSIsIlBSLUciLCJQUi1DIiwiUFItVSIsIlBSLUQiLCJQUlQtRyIsIlBSVC1DIiwiUFJULVUiLCJQUlQtRCIsIlRSLUciLCJUUi1DIiwiVFItVSIsIlRSLUQiLCJILUciLCJILUMiLCJILVUiLCJILUQiLCJPVFItRyIsIk9UUi1DIiwiT1RSLVUiLCJPVFItRCIsIk9UUlMtRyIsIk9UUlMtQyIsIk9UUlMtVSIsIk9UUlMtRCIsIldDLUciLCJXQy1DIiwiV0MtVSIsIldDLUQiLCJXQ1MtRyIsIldDUy1DIiwiV0NTLVUiLCJXQ1MtRCIsIkwtRyIsIkZOUC1HIiwiRk5QLVUiLCJGTlAtRCIsIkZOUC1DIiwiSUNQLUciLCJPQy1HIiwiV08tRyIsIkRELUciLCJERC1DIiwiREQtVSIsIkRELUQiLCJNLUciLCJEVFMtRyIsIklPVC1HIiwiVFMtRyIsIkZOLUciLCJGTi1DVCIsIk1QUi1HIiwiTVBSLUFVIiwiTFItRyIsIkNDLUciLCJDQy1DIiwiQ0MtVSIsIkNDLUQiLCJDQ1AtRyIsIkNDUC1DIiwiQ0NQLVUiLCJDQ1AtRCIsIlJRLUciLCJSUS1DSU9UIiwiUlEtR0lPVCIsIlJRLUNJQ1AiLCJSUS1HSUNQIiwiUlEtQ0wiLCJSUS1HTCIsIlJRLUNPQyIsIlJRLUdPQyIsIlJRLUNXTyIsIlJRLUdXTyIsIlJRLUNNIiwiUlEtR00iLCJXRi1HIiwiV0YtVSJdLCJuYmYiOjE2Nzg2MjUxMjEsImV4cCI6MTY3ODYyNjAyMX0.q2DQwqnKaRVA3ii46gc3QcTlK-mJOfA35Z26xm8Z9cA"
            };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, request.UserId.ToString()) }));
            _userContextServiceMock.Setup(x => x.CurrenConnection.RemoteIpAddress).Returns(null as System.Net.IPAddress);
            _userContextServiceMock.Setup(x => x.UserAgent).Returns("User-Agent");
            _refreshTokenRepositoryMock.Setup(x => x.GetLatestOneAsync(request.UserId, CancellationToken.None)).ReturnsAsync(new RefreshToken { ExpirationDate = DateTime.UtcNow.AddDays(1) });
            _userRepositoryMock.Setup(x => x.GetAsync(request.UserId, CancellationToken.None)).ReturnsAsync(null as User);
            _bearerTokensMock.Setup(x => x.CurrentValue).Returns(bearerToken);

            // Act
            Func<Task> action = async () => await _authenticationService.LogoutAsync(request, CancellationToken.None);

            // Assert
            await action.Should().ThrowAsync<BadRequestException>();
        }

        //[Fact]
        //public async Task RefreshTokenAsync_WithValidToken_ShouldReturnNewAccessTokenAndRefreshToken()
        //{
        //    // Arrange
        //    var userId = Guid.NewGuid();
        //    var accessToken = GenerateAccessToken(userId);
        //    var refreshToken = GenerateRefreshToken(userId);
        //    var user = new User { Id = userId, RefreshTokens = new List<RefreshToken> { refreshToken }, Groups = new List<Group>(), UserName = "username" };
        //    var request = new RefreshTokenQuery { AccessToken = accessToken, RefreshToken = refreshToken.Token };
        //    _userRepositoryMock.Setup(x => x.GetWithRoleAndRefreshTokensAsync(userId)).ReturnsAsync(user);
        //    _dateTimeServiceMock.Setup(x => x.Now).Returns(DateTime.UtcNow);
        //    _bearerTokensOptionsMock.Setup(x => x.CurrentValue.AccessTokenExpirationMinutes).Returns(30);

        //    // Act
        //    var result = await _authenticationService.RefreshTokenAsync(request);

        //    // Assert
        //    result.Token.Should().NotBeNullOrEmpty();
        //    result.RefreshToken.Should().NotBeNullOrEmpty();
        //    _tokenCacheServiceMock.Verify(x => x.AddOrUpdateAsync(userId, result.Token), Times.Once);
        //    _refreshTokenRepositoryMock.Verify(x => x.AddAsync(It.IsAny<RefreshToken>()), Times.Never);
        //}
    }
}
