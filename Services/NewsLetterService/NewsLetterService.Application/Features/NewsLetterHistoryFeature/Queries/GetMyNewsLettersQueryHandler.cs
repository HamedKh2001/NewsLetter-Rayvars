using AutoMapper;
using MediatR;
using NewsLetterService.Application.Contracts.Persistence;

namespace NewsLetterService.Application.Features.NewsLetterHistoryFeature.Queries
{
    internal class GetMyNewsLettersQueryHandler : IRequestHandler<GetMyNewsLettersQuery, List<NewsLetterHistoryDto>>
    {
        private readonly INewsLetterHistoryRepository _newsLetterHistoryRepository;
        private readonly IMapper _mapper;

        public GetMyNewsLettersQueryHandler(INewsLetterHistoryRepository newsLetterHistoryRepository, IMapper mapper)
        {
            _newsLetterHistoryRepository = newsLetterHistoryRepository;
            _mapper = mapper;
        }

        public async Task<List<NewsLetterHistoryDto>> Handle(GetMyNewsLettersQuery request, CancellationToken cancellationToken)
        {
            var result = await _newsLetterHistoryRepository.GetMyNewsLettersAsync(request.PersonnelId, request.Act, cancellationToken);
            return _mapper.Map<List<NewsLetterHistoryDto>>(result);
        }
    }
}
