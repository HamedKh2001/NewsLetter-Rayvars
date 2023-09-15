using CDN.Application.Contracts.Persistence;
using CDN.Domain.Entities;
using CDN.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CDN.Infrastructure.Repositories
{
    public class NewsLetterRepository : INewsLetterRepository
    {
        private readonly CDNDbContext _context;

        public NewsLetterRepository(CDNDbContext context)
        {
            _context = context;
        }

        public async Task<NewsLetter> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.NewsLetters.FindAsync(id, cancellationToken);
        }

        public NewsLetter GetWithCategory(int id)
        {
            return _context.NewsLetters.AsNoTracking().Include(f => f.Category).FirstOrDefault(f => f.Id == id);
        }

        public async Task<NewsLetter> CreateAsync(NewsLetter newsLetter, CancellationToken cancellationToken = default)
        {
            await _context.NewsLetters.AddAsync(newsLetter, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return newsLetter;
        }
    }
}
