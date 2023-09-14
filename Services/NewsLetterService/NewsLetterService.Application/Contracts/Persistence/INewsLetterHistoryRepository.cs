using NewsLetterService.Domain.Entities;
using static NewsLetterService.Domain.Enums.Act;

namespace NewsLetterService.Application.Contracts.Persistence
{
    public interface INewsLetterHistoryRepository
    {
        public Task<NewsLetterHistory> GetAsync(long id, CancellationToken cancellationToken = default);
        public Task<List<NewsLetterHistory>> GetMyNewsLettersAsync(int personnelId, ActType actType, CancellationToken cancellationToken = default);

        public Task<NewsLetterHistory> CreateAsync(NewsLetterHistory newsLetterHistory, CancellationToken cancellationToken = default);
    }
}
