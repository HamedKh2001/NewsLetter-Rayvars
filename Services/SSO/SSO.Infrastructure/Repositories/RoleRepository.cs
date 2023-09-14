using Microsoft.EntityFrameworkCore;
using SSO.Application.Contracts.Persistence;
using SSO.Domain.Entities;
using SSO.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly SSODbContext _context;

        public RoleRepository(SSODbContext context)
        {
            _context = context;
        }

        public async Task<List<Role>> GetAsync(CancellationToken cancellationToken)
        {
            return await _context.Roles.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<Role> GetAsync(long id, CancellationToken cancellationToken)
        {
            return await _context.Roles.FindAsync(id, cancellationToken);
        }

        public async Task<List<Role>> GetByRoleIdsAsync(List<long> ids, CancellationToken cancellationToken)
        {
            return await _context.Roles.AsNoTracking().Where(r => ids.Contains(r.Id)).Distinct().ToListAsync(cancellationToken);
        }

        public async Task<Role> GetWithGroupsAsync(long id, CancellationToken cancellationToken)
        {
            return await _context.Roles.AsNoTracking().Include(r => r.Groups).FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task<Role> GetWithUsersAsync(long id, CancellationToken cancellationToken)
        {
            return await _context.Roles.AsNoTracking().Include(r => r.Groups).ThenInclude(g => g.Users).FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task<bool> IsUniqueDisplayTitleAsync(long id, string displayTitle, CancellationToken cancellationToken)
        {
            return await _context.Roles.AnyAsync(r => r.DisplayTitle == displayTitle && r.Id != id, cancellationToken) == false;
        }

        public async Task UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            _context.SetModified(role);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
