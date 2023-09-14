using Microsoft.Extensions.DependencyInjection;
using SSO.Infrastructure.Persistence;
using System;

namespace SSO.Infrastructure.Middlewares
{
    public static class InfraMiddlewareExtension
    {
        public static void DbContextInitializer(this IServiceProvider service)
        {
            using (var scope = service.CreateScope())
            {
                var initialiser = scope.ServiceProvider.GetRequiredService<SSODbContextInitializer>();
                initialiser.Initialize();
                initialiser.Seed();
            }
        }
    }
}
