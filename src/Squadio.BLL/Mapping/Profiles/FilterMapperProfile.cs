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
                IncludeAdmin = false,
                IncludeDeleted = false
            });
            CreateMap<UserAdminFilterDTO, UserFilterModel>().ConvertUsing(x => new UserFilterModel
            {
                UserStatus = x.UserStatus,
                IncludeAdmin = x.IncludeAdmin,
                IncludeDeleted = x.IncludeDeleted
            });
        }
    }
}