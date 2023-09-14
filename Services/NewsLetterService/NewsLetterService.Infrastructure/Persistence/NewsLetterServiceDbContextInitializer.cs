using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace NewsLetterService.Infrastructure.Persistence
{
    public class NewsLetterServiceDbContextInitializer
    {
        private readonly ILogger<NewsLetterServiceDbContextInitializer> _logger;
        private readonly NewsLetterServiceDbContext _context;

        public NewsLetterServiceDbContextInitializer(ILogger<NewsLetterServiceDbContextInitializer> logger, NewsLetterServiceDbContext context)
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
