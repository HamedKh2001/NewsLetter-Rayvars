using Microsoft.EntityFrameworkCore;
using SharedKernel.Common;
using SharedKernel.Extensions;
using SSO.Application.Contracts.Persistence;
using SSO.Domain.Entities;
using SSO.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SSODbContext _context;

        public UserRepository(SSODbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<User>> GetAsync(string searchParam, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var query = _context.Users.AsNoTracking();

            if (string.IsNullOrEmpty(searchParam) == false)
                query = query.Where(
                    q => q.FirstName.Contains(searchParam) ||
                    q.LastName.Contains(searchParam));

            query = query.OrderBy(q => q.LastName);
            return await query.ToPagedListAsync(pageNumber, pageSize, cancellationToken);
        }

        public async Task<User> GetAsync(long id, CancellationToken cancellationToken)
        {
            return await _context.Users.FindAsync(id, cancellationToken);
        }

        public async Task<User> CreateAsync(User user, CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return user;
        }

        public async Task UpdateAsync(User user, CancellationToken cancellationToken)
        {
            _context.SetModified(user);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(User user, CancellationToken cancellationToken)
        {
            _context.Remove(user);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<User> GetUserWithRolesAsync(string userName, string encPass, CancellationToken cancellationToken)
        {
            return await _context.Users.Include(u => u.Groups).ThenInclude(g => g.Roles).FirstOrDefaultAsync(u => u.UserName == userName && u.Password == encPass, cancellationToken);
        }

        public async Task<User> GetUserByPasswordAsync(long userId, string encPass, CancellationToken cancellationToken)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.Password == encPass, cancellationToken);
        }

        public async Task<User> GetUserWithRolesAsync(long id, CancellationToken cancellationToken)
        {
            return await _context.Users.Include(u => u.Groups).ThenInclude(g => g.Roles).FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<User> GetWithRoleAndRefreshTokensAsync(long userId, CancellationToken cancellationToken)
        {
            return await _context.Users
                .Include(u => u.Groups)
                .ThenInclude(g => g.Roles)
                .Include(u => u.RefreshTokens.OrderByDescending(r => r.Id).Take(1))
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        }

        public async Task<List<User>> GetByUserIdsAsync(List<long> ids, CancellationToken cancellationToken)
        {
            return await _context.Users.Where(u => ids.Contains(u.Id)).Distinct().ToListAsync(cancellationToken);
        }

        public async Task<bool> IsUniqueUserNameAsync(string userName, CancellationToken cancellationToken)
        {
            return await _context.Users.AnyAsync(u => u.UserName == userName, cancellationToken) == false;
        }

        public async Task<User> GetUserWithGroupsAsync(long id, CancellationToken cancellationToken)
        {
            return await _context.Users.Include(u => u.Groups).FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<User> GetByUserNameAsync(string userName, CancellationToken cancellationToken)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
        }

        public async Task<bool> IsUniqueMobileAsync(string mobile, CancellationToken cancellationToken)
        {
            return await _context.Users.AnyAsync(u => u.Mobile == mobile, cancellationToken) == false;
        }

        public async Task<bool> IsUniqueMobileAsync(string mobile, long id, CancellationToken cancellationToken)
        {
            return await _context.Users.AnyAsync(u => u.Mobile == mobile && u.Id != id, cancellationToken) == false;
        }

        public async Task<PaginatedResult<User>> GetAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            return await _context.Users.AsNoTracking().ToPagedListAsync(pageNumber, pageSize, cancellationToken);
        }

        public async Task<PaginatedResult<User>> GetByGroupIdAsync(long groupId, string searchParam, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var query = _context.Users.Where(u => u.Groups.Any(g => g.Id == groupId)).AsNoTracking();

            if (string.IsNullOrEmpty(searchParam) == false)
                query = query.Where(
                    q => q.FirstName.Contains(searchParam) ||
                    q.LastName.Contains(searchParam));

            query = query.OrderBy(q => q.LastName);
            return await query.ToPagedListAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
