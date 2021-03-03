using System.Collections.Generic;
using AutoMapper;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Models.Companies;
using Squadio.DTO.Models.Resources;
using Squadio.DTO.Models.SignUp;
using Squadio.DTO.Models.Users;
using Squadio.DTO.Models.Users.Settings;

namespace Squadio.BLL.Mapping.Profiles
{
    public class UserMapperProfile: Profile
    {
        public UserMapperProfile()
        {
            CreateMap<UserModel, UserDTO>();
            CreateMap<UserModel, UserDetailDTO>();
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