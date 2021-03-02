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
            cfg.AddProfile<SignUpMapperProfile>();
            cfg.AddProfile<CompanyMapperProfile>();
            cfg.AddProfile<TeamMapperProfile>();
            cfg.AddProfile<ProjectMapperProfile>();
            cfg.AddProfile<InviteMapperProfile>();
            cfg.AddProfile<ResourceMapperProfile>();
        });

        public static IMapper GetMapper()
        {
            Config.AssertConfigurationIsValid();
            return Config.CreateMapper();
        }
    }
}