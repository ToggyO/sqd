using AutoMapper;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Models.Resources;
using Squadio.DTO.Models.SignUp;
using Squadio.DTO.Models.Users;

namespace Squadio.BLL.Mapping.Profiles
{
    public class UserMapperProfile: Profile
    {
        public UserMapperProfile()
        {
            CreateMap<UserModel, UserDTO>()
                .ForMember(
                    item => item.Avatar,
                    map => map.MapFrom(src => (ResourceImageDTO) null));
            CreateMap<UserModel, UserDetailDTO>()
                .ForMember(
                    item => item.Avatar,
                    map => map.MapFrom(src => (ResourceImageDTO) null));
            CreateMap<SignUpSimpleDTO, UserCreateDTO>().ConvertUsing(x => new UserCreateDTO
            {
                Email = x.Email,
                Name = x.Name,
                UserStatus = UserStatus.Active,
                SignUpBy = SignUpType.Email
            });
        }
    }
}