using CDN.Domain.Entities;

namespace CDN.Application.Contracts.Infrastructure
{
    public interface INewsLetterCacheService
    {
        NewsLetter Get(int newsLetterfileId);
        void Remove(int nNewsLetterId);
    }
}
