using CDN.Domain.Entities;

namespace CDN.Application.Contracts.Infrastructure
{
    public interface ICategoryCacheService
    {
        Category Get(int categoryId);
        void Remove(int categoryId);
    }
}
