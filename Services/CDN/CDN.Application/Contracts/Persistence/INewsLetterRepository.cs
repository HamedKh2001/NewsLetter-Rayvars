using CDN.Domain.Entities;

namespace CDN.Application.Contracts.Persistence
{
    public interface INewsLetterRepository
    {
        Task<NewsLetter> CreateAsync(NewsLetter newsLetter, CancellationToken cancellationToken = default);
        NewsLetter GetWithCategory(int id);
        Task<NewsLetter> GetAsync(int id, CancellationToken cancellationToken = default);
    }
}
