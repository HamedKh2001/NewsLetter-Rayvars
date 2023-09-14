using MediatR;
using static NewsLetterService.Domain.Enums.Act;

namespace NewsLetterService.Application.Features.NewsLetterHistoryFeature.Queries
{
    public class GetMyNewsLettersQuery : IRequest<List<NewsLetterHistoryDto>>
    {
        public int PersonnelId { get; set; }
        public ActType Act { get; set; }
    }
}
