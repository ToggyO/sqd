using System.Collections.Generic;
using AutoMapper;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Models.Admin;
using Squadio.DTO.Models.Companies;
using Squadio.DTO.Models.Users;
using Squadio.DTO.Models.Users.Settings;

namespace Squadio.BLL.Mapping.Profiles
{
    public class UserMapperProfile: Profile
    {
        public UserMapperProfile()
        {
            CreateMap<UserModel, UserDTO>();
            CreateMap<UserModel, UserDetailAdminDTO>()
                .ForMember(
                    item => item.RoleId,
                    map => map.MapFrom(src => src.RoleId))
                .ForMember(
                    item => item.RoleName,
                    map => map.MapFrom(src => src.Role.Name));
            CreateMap<UserConfirmEmailRequestModel, UserConfirmEmailRequestDTO>();
            CreateMap<UserModel, UserWithCompaniesDTO>()
                .ForMember(
                    item => item.User,
                    map => map.MapFrom(src => src))
                .ForMember(
                    item => item.Companies,
                    map => map.MapFrom(src => new List<CompanyWithUserRoleDTO>()));
        }
    }
}