using AutoMapper;
using NewsLetterService.Application.Features.NewsLetterHistoryFeature.Commands.CreateNewsLetterHistory;
using NewsLetterService.Application.Features.NewsLetterHistoryFeature.Queries;
using NewsLetterService.Application.Features.PersonnelFeature.Commands;
using NewsLetterService.Application.Features.PersonnelFeature.Queries.GetPersonnels;
using NewsLetterService.Domain.Entities;
using SharedKernel.Common;

namespace NewsLetterService.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap(typeof(PaginatedResult<>), typeof(PaginatedList<>));

            CreateMap<Personnel, PersonnelDto>();
            CreateMap<CreatePersonnelCommand, Personnel>();

            CreateMap<CreateNewsLetterHistoryCommand, NewsLetterHistory>();
            CreateMap<NewsLetterHistory, NewsLetterHistoryDto>();
        }
    }
}
