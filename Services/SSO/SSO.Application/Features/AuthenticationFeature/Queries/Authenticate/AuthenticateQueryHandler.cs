using MediatR;
using SSO.Application.Contracts.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.Application.Features.AuthenticationFeature.Queries.Authenticate
{
    public class AuthenticateQueryHandler : IRequestHandler<AuthenticateQuery, AuthenticateDto>
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticateQueryHandler(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<AuthenticateDto> Handle(AuthenticateQuery request, CancellationToken cancellationToken)
        {
            var resault = await _authenticationService.AuthenticateAsync(request, cancellationToken);
            return resault;
        }
    }
}
