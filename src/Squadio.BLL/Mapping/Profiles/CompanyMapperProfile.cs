using System.Collections.Generic;
using AutoMapper;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Companies;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Models.Admin;
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
            CreateMap<CompanyModel, CompanyDetailAdminDTO>()
                .ForMember(
                    item => item.Admins,
                    map => map.MapFrom(src => new List<UserWithRoleDTO>()))
                .ForMember(
                    item => item.UsersCount,
                    map => map.MapFrom(src => 0));
            CreateMap<CompanyUserModel, CompanyWithUserRoleDTO>()
                .ForMember(
                    item => item.MembershipStatus,
                    map => map.MapFrom(src => src.Status))
                .ForMember(
                    item => item.CompanyName,
                    map => map.MapFrom(src => src.Company.Name));
            CreateMap<CompanyUserModel, UserWithRoleDTO>()
                .ForMember(
                    item => item.MembershipStatus,
                    map => map.MapFrom(src => src.Status));
        }
    }
}