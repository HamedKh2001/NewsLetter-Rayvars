using AutoMapper;
using SSO.Application.Features.UserFeature.Queries.GetUser;
using SSO.Application.Mapping;
using SSO.Domain.Entities;

namespace SSO.Application.UnitTests.Common
{
    public static class HelperExtensions
    {
        public static IMapper GetMapper(this IMapper mapper)
        {
            var configurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
                cfg.CreateMap<User, UserDto>()
                 .ForMember(u => u.Gender, opt => opt.MapFrom(src => (byte)src.Gender));
            });

            return configurationProvider.CreateMapper();
        }
    }
}
