using Microsoft.EntityFrameworkCore;
using SharedKernel.Common;
using SharedKernel.Extensions;
using SSO.Application.Contracts.Persistence;
using SSO.Domain.Entities;
using SSO.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.Infrastructure.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly SSODbContext _context;

        public GroupRepository(SSODbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<Group>> GetAsync(string caption, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var query = _context.Groups.AsNoTracking();
            if (string.IsNullOrEmpty(caption) == false)
                query = query.Where(q => q.Caption.Contains(caption));

            return await query.ToPagedListAsync(pageNumber, pageSize, cancellationToken);
        }

        public async Task<List<Group>> GetAsync(CancellationToken cancellationToken)
        {
            return await _context.Groups.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<Group> GetAsync(long id, CancellationToken cancellationToken)
        {
            return await _context.Groups.FindAsync(id, cancellationToken);
        }

        public async Task<Group> GetWithRolesAsync(long id, CancellationToken cancellationToken)
        {
            return await _context.Groups.Include(g => g.Roles).FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
        }

        public async Task<Group> GetWithUsersAsync(long id, CancellationToken cancellationToken)
        {
            return await _context.Groups.Include(g => g.Users).FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
        }

        public async Task<Group> CreateAsync(Group group, CancellationToken cancellationToken)
        {
            await _context.Groups.AddAsync(group, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return group;
        }

        public async Task UpdateAsync(Group group, CancellationToken cancellationToken)
        {
            _context.SetModified(group);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public Task DeleteAsync(Group group, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Group>> GetByIdsAsync(List<long> ids, CancellationToken cancellationToken)
        {
            return await _context.Groups.Where(g => ids.Contains(g.Id)).ToListAsync(cancellationToken);
        }

        public async Task<bool> IsUniqueCaptionAsync(string caption, CancellationToken cancellationToken)
        {
            return await _context.Groups.AnyAsync(g => g.Caption == caption, cancellationToken) == false;
        }

        public async Task<bool> IsUniqueCaptionAsync(long id, string caption, CancellationToken cancellationToken)
        {
            return await _context.Groups.AnyAsync(g => g.Caption == caption && g.Id != id, cancellationToken) == false;
        }
    }
}
