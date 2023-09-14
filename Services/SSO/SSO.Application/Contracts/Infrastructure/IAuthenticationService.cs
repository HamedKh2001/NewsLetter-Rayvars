using SSO.Application.Features.AuthenticationFeature.Commands.ChangePassword;
using SSO.Application.Features.AuthenticationFeature.Commands.UpdateUserGroup;
using SSO.Application.Features.AuthenticationFeature.Queries.Authenticate;
using SSO.Application.Features.AuthenticationFeature.Queries.LogoutUser;
using SSO.Application.Features.AuthenticationFeature.Queries.RefreshToken;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.Application.Contracts.Infrastructure
{
    public interface IAuthenticationService
    {
        Task<AuthenticateDto> AuthenticateAsync(AuthenticateQuery request, CancellationToken cancellationToken);
        Task ChangePasswordAsync(ChangePasswordCommand request, CancellationToken cancellationToken);
        Task LogoutAsync(LogoutUserQuery request, CancellationToken cancellationToken);
        Task<AuthenticateDto> RefreshTokenAsync(RefreshTokenQuery request, CancellationToken cancellationToken);
        Task UpdateUserGroupAsync(UpdateUserGroupCommand request, CancellationToken cancellationToken);
    }
}
