using CDN.Domain.Entities;

namespace CDN.Application.Contracts.Infrastructure
{
    public interface ICategoryCacheService
    {
        Category Get(long categoryId);
        void Remove(long categoryId);
    }
}
