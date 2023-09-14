using AutoMapper;
using MediatR;
using NewsLetterService.Application.Contracts.Persistence;
using SharedKernel.Common;

namespace NewsLetterService.Application.Features.PersonnelFeature.Queries.GetPersonnels
{
    public class GetPersonnelsQueryHandler : IRequestHandler<GetPersonnelsQuery, PaginatedList<PersonnelDto>>
    {
        private readonly IPersonnelRepository _personnelRepository;
        private readonly IMapper _mapper;

        public GetPersonnelsQueryHandler(IPersonnelRepository personnelRepository, IMapper mapper)
        {
            _personnelRepository = personnelRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedList<PersonnelDto>> Handle(GetPersonnelsQuery request, CancellationToken cancellationToken)
        {
            var personnels = await _personnelRepository.GetAsync(request.PageNumber,
                                                               request.PageSize,
                                                               cancellationToken);

            return _mapper.Map<PaginatedList<PersonnelDto>>(personnels);
        }
    }
}
