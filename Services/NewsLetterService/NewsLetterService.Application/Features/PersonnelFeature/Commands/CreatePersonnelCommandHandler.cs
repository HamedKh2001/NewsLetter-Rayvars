using AutoMapper;
using MediatR;
using NewsLetterService.Application.Contracts.Persistence;
using NewsLetterService.Application.Features.PersonnelFeature.Queries.GetPersonnels;
using NewsLetterService.Domain.Entities;

namespace NewsLetterService.Application.Features.PersonnelFeature.Commands
{
    public class CreatePersonnelCommandHandler : IRequestHandler<CreatePersonnelCommand, PersonnelDto>
    {
        private readonly IPersonnelRepository _personnelRepository;
        private readonly IMapper _mapper;

        public CreatePersonnelCommandHandler(IPersonnelRepository personnelRepository, IMapper mapper)
        {
            _personnelRepository = personnelRepository;
            _mapper = mapper;
        }

        public async Task<PersonnelDto> Handle(CreatePersonnelCommand request, CancellationToken cancellationToken)
        {
            var newPersonnel = _mapper.Map<Personnel>(request);

            var result = await _personnelRepository.CreateAsync(newPersonnel, cancellationToken);

            return _mapper.Map<PersonnelDto>(result);
        }
    }
}
