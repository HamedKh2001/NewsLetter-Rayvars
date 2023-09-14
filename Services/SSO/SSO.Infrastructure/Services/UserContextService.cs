using Microsoft.AspNetCore.Http;
using SharedKernel.Common;
using SharedKernel.Extensions;
using SSO.Application.Contracts.Infrastructure;
using System.Linq;

namespace SSO.Infrastructure.Services
{
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public UserIdentitySharedModel CurrentUser => GetCurrentUser();

        public ConnectionInfo CurrenConnection => _httpContextAccessor.HttpContext.Connection;

        public string UserAgent => GetUserAgent();

        private string GetUserAgent()
        {
            var userAgent = _httpContextAccessor.HttpContext.Request.Headers?.FirstOrDefault(s => s.Key.ToLower() == "user-agent");

            if (userAgent.HasValue == false)
                return string.Empty;

            return userAgent.Value.Value;
        }

        private UserIdentitySharedModel GetCurrentUser()
        {
            var user = _httpContextAccessor.HttpContext.User;
            return new UserIdentitySharedModel
            {
                Id = user.Identity.GetUserId(),
                UserName = user.Identity.GetUserName(),
                FirstName = user.Identity.GetUserFirstName(),
                LastName = user.Identity.GetUserLastName()
            };
        }
    }
}
