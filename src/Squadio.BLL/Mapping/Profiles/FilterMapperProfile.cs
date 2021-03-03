using AutoMapper;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Domain.Enums;
using Squadio.DTO.Models.Admin;
using Squadio.DTO.Models.Users;

namespace Squadio.BLL.Mapping.Profiles
{
    public class FilterMapperProfile : Profile
    {
        public FilterMapperProfile()
        {
            CreateMap<UserFilterDTO, UserFilterModel>()
                .ForMember(
                    item => item.UserStatus,
                    map => map.MapFrom(src => (UserStatus?) null))
                .ForMember(
                    item => item.IncludeAdmin,
                    map => map.MapFrom(src => false))
                .ForMember(
                    item => item.IncludeDeleted,
                    map => map.MapFrom(src => false));
            CreateMap<UserFilterAdminDTO, UserFilterModel>();
            CreateMap<CompanyFilterAdminDTO, CompanyFilterModel>();
        }
    }
}