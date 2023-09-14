using MediatR;
using NewsLetterService.Application.Features.PersonnelFeature.Queries.GetPersonnels;
using SharedKernel.Common;

namespace NewsLetterService.Application.Features.PersonnelFeature.Commands
{
    public class CreatePersonnelCommand : IRequest<PersonnelDto>
    {
        public string Name { get; set; }
        public string NationalCode { get; set; }
    }
}
