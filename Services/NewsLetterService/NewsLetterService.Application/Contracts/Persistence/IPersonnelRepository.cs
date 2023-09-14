using Microsoft.EntityFrameworkCore;
using NewsLetterService.Domain.Entities;
using SharedKernel.Common;

namespace NewsLetterService.Application.Contracts.Persistence
{
    public interface IPersonnelRepository
    {
        public Task<Personnel> GetAsync(long id, CancellationToken cancellationToken = default);

        public Task<PaginatedResult<Personnel>> GetAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
              
        public Task<Personnel> CreateAsync(Personnel personnel, CancellationToken cancellationToken = default);
        public Task UpdateAsync(Personnel personnel, CancellationToken cancellationToken = default);
    }
}
