using Microsoft.AspNetCore.Http;
using SharedKernel.Common;

namespace SSO.Application.Contracts.Infrastructure
{
    public interface IUserContextService
    {
        UserIdentitySharedModel CurrentUser { get; }
        ConnectionInfo CurrenConnection { get; }
        string UserAgent { get; }
    }
}
