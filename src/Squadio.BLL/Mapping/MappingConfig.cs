using AutoMapper;
using Squadio.BLL.Mapping.Profiles;

namespace Squadio.BLL.Mapping
{
    public class MappingConfig
    {
        private static readonly MapperConfiguration Config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PageMapperProfile>();
        });

        public static IMapper GetMapper()
        {
            Config.AssertConfigurationIsValid();
            return Config.CreateMapper();
        }
    }
}