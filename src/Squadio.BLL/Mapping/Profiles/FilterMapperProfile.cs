using AutoMapper;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.DTO.Models.Users;

namespace Squadio.BLL.Mapping.Profiles
{
    public class FilterMapperProfile : Profile
    {
        public FilterMapperProfile()
        {
            CreateMap<UserFilterDTO, UserFilterModel>().ConvertUsing(x => new UserFilterModel
            {
                Search = x.Search,
                IncludeAdmin = false,
                IncludeDeleted = false
            });
            CreateMap<UserFilterAdminDTO, UserFilterModel>().ConvertUsing(x => new UserFilterModel
            {
                Search = x.Search,
                UserStatus = x.UserStatus,
                IncludeAdmin = x.IncludeAdmin,
                IncludeDeleted = x.IncludeDeleted
            });
        }
    }
}