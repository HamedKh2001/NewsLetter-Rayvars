using CDN.Domain.Entities;

namespace CDN.Application.Contracts.Infrastructure
{
    public interface INewsLetterCacheService
    {
        NewsLetter Get(long newsLetterfileId);
        void Remove(long nNewsLetterId);
    }
}
