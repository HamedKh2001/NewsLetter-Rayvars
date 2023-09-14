using MediatR;
using SharedKernel.Common;

namespace NewsLetterService.Application.Features.PersonnelFeature.Queries.GetPersonnels
{
    public class GetPersonnelsQuery : PaginationQuery, IRequest<PaginatedList<PersonnelDto>>
    {
    }
}
