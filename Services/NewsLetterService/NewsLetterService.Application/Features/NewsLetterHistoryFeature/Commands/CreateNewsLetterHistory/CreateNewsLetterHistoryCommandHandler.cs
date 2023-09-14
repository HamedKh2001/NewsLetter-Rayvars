using AutoMapper;
using MediatR;
using NewsLetterService.Application.Contracts.Persistence;
using NewsLetterService.Domain.Entities;
using SharedKernel.Contracts.Infrastructure;

namespace NewsLetterService.Application.Features.NewsLetterHistoryFeature.Commands.CreateNewsLetterHistory
{
    public class CreateNewsLetterHistoryCommandHandler : IRequestHandler<CreateNewsLetterHistoryCommand>
    {
        private readonly INewsLetterHistoryRepository _newsLetterHistoryRepository;
        private readonly IMapper _mapper;
        private readonly IDateTimeService _dateTimeService;

        public CreateNewsLetterHistoryCommandHandler(INewsLetterHistoryRepository newsLetterHistoryRepository, IMapper mapper, IDateTimeService dateTimeService)
        {
            _newsLetterHistoryRepository = newsLetterHistoryRepository;
            _mapper = mapper;
            _dateTimeService = dateTimeService;
        }

        public async Task<Unit> Handle(CreateNewsLetterHistoryCommand request, CancellationToken cancellationToken)
        {
            var newNewsLetterHistory = _mapper.Map<NewsLetterHistory>(request);
            newNewsLetterHistory.DateOfAct = _dateTimeService.Now;

            await _newsLetterHistoryRepository.CreateAsync(newNewsLetterHistory, cancellationToken);

            return Unit.Value;
        }
    }
}
