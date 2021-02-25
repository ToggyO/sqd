using AutoMapper;
using Squadio.BLL.Mapping.Profiles;

namespace Squadio.BLL.Mapping
{
    public class MappingConfig
    {
        private static readonly MapperConfiguration Config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<FilterMapperProfile>();
            cfg.AddProfile<PageMapperProfile>();
            cfg.AddProfile<UserMapperProfile>();
        });

        public static IMapper GetMapper()
        {
            Config.AssertConfigurationIsValid();
            return Config.CreateMapper();
        }
    }
}