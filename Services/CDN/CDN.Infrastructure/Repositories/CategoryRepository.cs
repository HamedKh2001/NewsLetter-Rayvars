using CDN.Application.Contracts.Persistence;
using CDN.Domain.Entities;
using CDN.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common;
using SharedKernel.Extensions;

namespace CDN.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CDNDbContext _context;

        public CategoryRepository(CDNDbContext context)
        {
            _context = context;
        }

        public async Task<Category> GetAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _context.Categories.FindAsync(id, cancellationToken);
        }

        public Category Get(long id)
        {
            return _context.Categories.Find(id);
        }

        public async Task<PaginatedResult<Category>> GetAsync(bool? isActive, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _context.Categories.AsNoTracking();

            if (isActive.HasValue == true)
                query = query.Where(q => q.IsActive == isActive.Value);

            query = query.OrderByDescending(q => q.Id);

            return await query.ToPagedListAsync(pageNumber, pageSize, cancellationToken);
        }

        public async Task<Category> CreateAsync(Category category, CancellationToken cancellationToken = default)
        {
            await _context.Categories.AddAsync(category, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return category;
        }

        public async Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
        {
            _context.SetModified(category);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> IsUniqueTitleAsync(string title, CancellationToken cancellationToken = default)
        {
            return await _context.Categories.AnyAsync(c => c.Title == title, cancellationToken) == false;
        }

        public async Task<bool> IsUniqueTitleAsync(string title, long id, CancellationToken cancellationToken = default)
        {
            return await _context.Categories.AnyAsync(c => c.Title == title && c.Id != id, cancellationToken) == false;
        }
    }
}
