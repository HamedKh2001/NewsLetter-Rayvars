using MediatR;
using static NewsLetterService.Domain.Enums.Act;

namespace NewsLetterService.Application.Features.NewsLetterHistoryFeature.Commands.CreateNewsLetterHistory
{
    public class CreateNewsLetterHistoryCommand : IRequest
    {
        public int PersonnelId { get; set; }
        public int NewsLetterId { get; set; }
        public ActType Act { get; set; }
    }
}
