using Microsoft.EntityFrameworkCore;
using SharedKernel.Common;
using SharedKernel.Extensions;
using SSO.Application.Contracts.Persistence;
using SSO.Domain.Entities;
using SSO.Infrastructure.Persistence;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.Infrastructure.Repositories
{
    public class UserLoginRepository : IUserLoginRepository
    {
        private readonly SSODbContext _context;

        public UserLoginRepository(SSODbContext context)
        {
            _context = context;
        }

        public async Task<UserLogin> CreateAsync(UserLogin userLogin, CancellationToken cancellationToken)
        {
            await _context.UserLogins.AddAsync(userLogin, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return userLogin;
        }

        public async Task<PaginatedResult<UserLogin>> GetAsync(int userId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var query = _context.UserLogins.AsNoTracking().Include(u => u.User).Where(u => u.UserId == userId);
            query = query.OrderBy(q => q.Id);
            return await query.ToPagedListAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
