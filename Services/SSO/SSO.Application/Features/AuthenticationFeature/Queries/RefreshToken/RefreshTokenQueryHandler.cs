using MediatR;
using SSO.Application.Contracts.Infrastructure;
using SSO.Application.Features.AuthenticationFeature.Queries.Authenticate;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.Application.Features.AuthenticationFeature.Queries.RefreshToken
{
    public class RefreshTokenQueryHandler : IRequestHandler<RefreshTokenQuery, AuthenticateDto>
    {
        private readonly IAuthenticationService _authenticationService;

        public RefreshTokenQueryHandler(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<AuthenticateDto> Handle(RefreshTokenQuery request, CancellationToken cancellationToken)
        {
            return await _authenticationService.RefreshTokenAsync(request, cancellationToken);
        }
    }
}
