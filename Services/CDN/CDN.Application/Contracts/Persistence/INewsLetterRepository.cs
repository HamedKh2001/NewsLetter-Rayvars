using CDN.Domain.Entities;

namespace CDN.Application.Contracts.Persistence
{
    public interface INewsLetterRepository
    {
        Task<NewsLetter> CreateAsync(NewsLetter newsLetter, CancellationToken cancellationToken = default);
        NewsLetter GetWithCategory(long id);
        Task<NewsLetter> GetAsync(long id, CancellationToken cancellationToken = default);
    }
}
