using CDN.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace CDN.Infrastructure.Middlewares
{
    public static class InfraMiddlewareExtension
    {
        public static void DbContextInitializer(this IServiceProvider service)
        {
            using (var scope = service.CreateScope())
            {
                var initialiser = scope.ServiceProvider.GetRequiredService<CDNDbContextInitializer>();
                initialiser.Initialize();
            }
        }
    }
}
