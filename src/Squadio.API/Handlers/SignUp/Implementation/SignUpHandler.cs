using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.BLL.Providers.SignUp;
using Squadio.BLL.Services.SignUp;
using Squadio.BLL.Services.Tokens;
using Squadio.Common.Extensions;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Auth;
using Squadio.DTO.Companies;
using Squadio.DTO.Projects;
using Squadio.DTO.SignUp;
using Squadio.DTO.Teams;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.SignUp.Implementation
{
    public class SignUpHandler : ISignUpHandler
    {
        private readonly ISignUpProvider _provider;
        private readonly ISignUpService _service;
        private readonly ITokensService _tokensService;
        public SignUpHandler(ISignUpProvider provider
            , ISignUpService service
            , ITokensService tokensService)
        {
            _provider = provider;
            _service = service;
            _tokensService = tokensService;
        }

        public async Task<Response<UserRegistrationStepDTO>> GetRegistrationStep(string email)
        {
            var result = await _provider.GetRegistrationStep(email);
            return result;
        }

        public async Task<Response<AuthInfoDTO>> SignUpMemberEmail(SignUpMemberDTO dto)
        {
            var signUpResult = await _service.SignUpMemberEmail(dto);
            if (!signUpResult.IsSuccess)
            {
                return new ErrorResponse<AuthInfoDTO>
                {
                    Message = ((ErrorResponse<UserDTO>) signUpResult).Message,
                    HttpStatusCode = signUpResult.HttpStatusCode,
                    Code = signUpResult.Code
                };
            }
            
            var result = await _tokensService.Authenticate(new CredentialsDTO
            {
                Password = dto.Password,
                Email = dto.Email
            });
            return result;
        }

        public async Task<Response<AuthInfoDTO>> SignUpMemberGoogle(SignUpMemberGoogleDTO dto)
        {
            var signUpResult = await _service.SignUpMemberGoogle(dto);
            if (!signUpResult.IsSuccess)
            {
                return new ErrorResponse<AuthInfoDTO>
                {
                    Message = ((ErrorResponse<UserDTO>) signUpResult).Message,
                    HttpStatusCode = signUpResult.HttpStatusCode,
                    Code = signUpResult.Code
                };
            }
            
            var result = await _tokensService.GoogleAuthenticate(dto.Token);
            return result;
        }

        public async Task<Response> SignUp(string email, string password)
        {
            var result = await _service.SignUp(email, password);
            return result;
        }

        public async Task<Response<UserDTO>> SignUpGoogle(string googleToken)
        {
            var result = await _service.SignUpGoogle(googleToken);
            return result;
        }

        /*
        public async Task<Response<AuthInfoDTO>> SignUpPassword(UserSetPasswordDTO dto)
        {
            var signUpPasswordResult = await _service.SignUpPassword(dto.Email, dto.Code, dto.Password);
            if (!signUpPasswordResult.IsSuccess)
            {
                return new ErrorResponse<AuthInfoDTO>
                {
                    Message = ((ErrorResponse<UserDTO>) signUpPasswordResult).Message,
                    HttpStatusCode = signUpPasswordResult.HttpStatusCode,
                    Code = signUpPasswordResult.Code
                };
            }
            
            var result = await _tokensService.Authenticate(new CredentialsDTO
            {
                Password = dto.Password,
                Email = dto.Email
            });
            return result;
        }
        */

        public async Task<Response<UserDTO>> SignUpUsername(UserUpdateDTO dto, ClaimsPrincipal claims)
        {
            var result = await _service.SignUpUsername(claims.GetUserId(), dto);
            return result;
        }

        public async Task<Response> SignUpAgreement(ClaimsPrincipal claims)
        {
            var result = await _service.SignUpAgreement(claims.GetUserId());
            return result;
        }

        public async Task<Response<CompanyDTO>> SignUpCompany(CreateCompanyDTO dto, ClaimsPrincipal claims)
        {
            var result = await _service.SignUpCompany(claims.GetUserId(), dto);
            return result;
        }

        public async Task<Response<TeamDTO>> SignUpTeam(CreateTeamDTO dto, ClaimsPrincipal claims)
        {
            var result = await _service.SignUpTeam(claims.GetUserId(), dto);
            return result;
        }

        public async Task<Response<ProjectDTO>> SignUpProject(CreateProjectDTO dto, ClaimsPrincipal claims)
        {
            var result = await _service.SignUpProject(claims.GetUserId(), dto);
            return result;
        }

        public async Task<Response> SignUpDone(ClaimsPrincipal claims)
        {
            var result = await _service.SignUpDone(claims.GetUserId());
            return result;
        }
    }
}