using AutoMapper;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Models.Resources;
using Squadio.DTO.Models.SignUp;
using Squadio.DTO.Models.Users;
using Squadio.DTO.Models.Users.Settings;

namespace Squadio.BLL.Mapping.Profiles
{
    public class SignUpMapperProfile: Profile
    {
        public SignUpMapperProfile()
        {
            CreateMap<SignUpSimpleDTO, UserCreateDTO>().ConvertUsing(x => new UserCreateDTO
            {
                Email = x.Email,
                Name = x.Name,
                UserStatus = UserStatus.Active,
                SignUpBy = SignUpType.Email
            });
            CreateMap<UserRegistrationStepModel, UserRegistrationStepDTO>().ConvertUsing(x => new UserRegistrationStepDTO
            {
                Step = (int) x.Step,
                StepName = x.Step.ToString(),
                Status = (int) x.Status,
                StatusName = x.Status.ToString()
            });
            CreateMap<UserRegistrationStepModel, SignUpStepDTO>()
                .ForMember(
                    item => item.RegistrationStep,
                    map => map.MapFrom(src => src));
        }
    }
}