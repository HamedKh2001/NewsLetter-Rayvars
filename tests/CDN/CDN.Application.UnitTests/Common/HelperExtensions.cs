using AutoMapper;
using CDN.Application.Mapping;

namespace CDN.Application.UnitTests.Common
{
    public static class HelperExtensions
    {
        public static IMapper GetMapper(this IMapper mapper)
        {
            var configurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            return configurationProvider.CreateMapper();
        }
    }
}
