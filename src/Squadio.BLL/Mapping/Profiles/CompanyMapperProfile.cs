using AutoMapper;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Companies;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Models.Companies;
using Squadio.DTO.Models.Resources;
using Squadio.DTO.Models.SignUp;
using Squadio.DTO.Models.Users;
using Squadio.DTO.Models.Users.Settings;

namespace Squadio.BLL.Mapping.Profiles
{
    public class CompanyMapperProfile: Profile
    {
        public CompanyMapperProfile()
        {
            CreateMap<CompanyModel, CompanyDTO>();
            CreateMap<CompanyUserModel, CompanyWithUserRoleDTO>()
                .ForMember(
                    item => item.Status,
                    map => map.MapFrom(src => (int) src.Status))
                .ForMember(
                    item => item.StatusName,
                    map => map.MapFrom(src => src.Status.ToString()));
        }
    }
}