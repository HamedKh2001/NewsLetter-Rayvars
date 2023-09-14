using Microsoft.EntityFrameworkCore;
using NewsLetterService.Application.Contracts.Persistence;
using NewsLetterService.Domain.Entities;
using NewsLetterService.Infrastructure.Persistence;
using static NewsLetterService.Domain.Enums.Act;

namespace NewsLetterService.Infrastructure.Repositories
{
    public class NewsLetterHistoryRepository : INewsLetterHistoryRepository
    {
        private readonly NewsLetterServiceDbContext _context;

        public NewsLetterHistoryRepository(NewsLetterServiceDbContext context)
        {
            _context = context;
        }

        public async Task<NewsLetterHistory> GetAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _context.NewsLetterHistories.FindAsync(id, cancellationToken);
        }

        public async Task<NewsLetterHistory> CreateAsync(NewsLetterHistory newsLetterHistory, CancellationToken cancellationToken = default)
        {
            await _context.NewsLetterHistories.AddAsync(newsLetterHistory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return newsLetterHistory;
        }

        public async Task<List<NewsLetterHistory>> GetMyNewsLettersAsync(int personnelId, ActType actType, CancellationToken cancellationToken = default)
        {
            return await _context.NewsLetterHistories.Where(nlh => nlh.PersonnelId == personnelId && nlh.Act == actType).ToListAsync(cancellationToken);
        }
    }
}
