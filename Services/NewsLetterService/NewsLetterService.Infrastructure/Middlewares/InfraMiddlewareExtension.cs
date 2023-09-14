using Microsoft.Extensions.DependencyInjection;
using NewsLetterService.Infrastructure.Persistence;

namespace NewsLetterService.Infrastructure.Middlewares
{
    public static class InfraMiddlewareExtension
    {
        public static void DbContextInitializer(this IServiceProvider service)
        {
            using (var scope = service.CreateScope())
            {
                var initialiser = scope.ServiceProvider.GetRequiredService<NewsLetterServiceDbContextInitializer>();
                initialiser.Initialize();
            }
        }
    }
}
