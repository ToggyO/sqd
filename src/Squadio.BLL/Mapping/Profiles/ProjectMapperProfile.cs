using AutoMapper;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Companies;
using Squadio.Domain.Models.Projects;
using Squadio.Domain.Models.Teams;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Models.Companies;
using Squadio.DTO.Models.Projects;
using Squadio.DTO.Models.Resources;
using Squadio.DTO.Models.SignUp;
using Squadio.DTO.Models.Teams;
using Squadio.DTO.Models.Users;
using Squadio.DTO.Models.Users.Settings;

namespace Squadio.BLL.Mapping.Profiles
{
    public class ProjectMapperProfile: Profile
    {
        public ProjectMapperProfile()
        {
            CreateMap<ProjectModel, ProjectDTO>();
            CreateMap<ProjectUserModel, ProjectWithUserRoleDTO>()
                .ForMember(
                    item => item.MembershipStatus,
                    map => map.MapFrom(src => src.Status));
            CreateMap<ProjectUserModel, UserWithRoleDTO>()
                .ForMember(
                    item => item.MembershipStatus,
                    map => map.MapFrom(src => src.Status));
        }
    }
}