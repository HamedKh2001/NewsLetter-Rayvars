using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CDN.Infrastructure.Persistence
{
    public class CDNDbContextInitializer
    {
        private readonly ILogger<CDNDbContextInitializer> _logger;
        private readonly CDNDbContext _context;

        public CDNDbContextInitializer(ILogger<CDNDbContextInitializer> logger, CDNDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void Initialize()
        {
            try
            {
                _context.Database.Migrate();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initialising the database.");
                throw;
            }
        }
    }
}
