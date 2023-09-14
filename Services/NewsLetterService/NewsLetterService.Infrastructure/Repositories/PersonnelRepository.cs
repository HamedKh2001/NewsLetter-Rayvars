using Microsoft.EntityFrameworkCore;
using NewsLetterService.Application.Contracts.Persistence;
using NewsLetterService.Domain.Entities;
using NewsLetterService.Infrastructure.Persistence;
using SharedKernel.Common;
using SharedKernel.Extensions;

namespace NewsLetterService.Infrastructure.Repositories
{
    public class PersonnelRepository : IPersonnelRepository
    {
        private readonly NewsLetterServiceDbContext _context;

        public PersonnelRepository(NewsLetterServiceDbContext context)
        {
            _context = context;
        }

        public async Task<Personnel> GetAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _context.Personnels.FindAsync(id, cancellationToken);
        }

        public async Task<PaginatedResult<Personnel>> GetAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _context.Personnels.AsNoTracking();

            query = query.OrderByDescending(q => q.Id);

            return await query.ToPagedListAsync(pageNumber, pageSize, cancellationToken);
        }

        public async Task<Personnel> CreateAsync(Personnel personnel, CancellationToken cancellationToken = default)
        {
            await _context.Personnels.AddAsync(personnel, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return personnel;
        }

        public async Task UpdateAsync(Personnel personnel, CancellationToken cancellationToken = default)
        {
            _context.SetModified(personnel);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
