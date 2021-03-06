using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.BLL.Providers.SignUp;
using Squadio.BLL.Services.SignUp;
using Squadio.BLL.Services.Tokens;
using Squadio.Common.Extensions;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Models.Auth;
using Squadio.DTO.Models.Companies;
using Squadio.DTO.Models.Projects;
using Squadio.DTO.Models.SignUp;
using Squadio.DTO.Models.Teams;
using Squadio.DTO.Models.Users;
using Squadio.DTO.Models.Users.Settings;
using Squadio.DTO.Projects;

namespace Squadio.API.Handlers.SignUp.Implementations
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
        
        public async Task<Response> SimpleSignUp(SignUpSimpleDTO dto)
        {
            return await _service.SimpleSignUp(dto);
        }

        public async Task<Response<SignUpStepDTO>> GetRegistrationStep(ClaimsPrincipal claims)
        {
            var result = await _provider.GetRegistrationStep(claims.GetUserId());
            return result;
        }

        public async Task<Response<AuthInfoDTO>> SignUpMemberEmail(SignUpMemberDTO dto)
        {
            var signUpResult = await _service.SignUpMemberEmail(dto);
            if (!signUpResult.IsSuccess)
            {
                var errorResponse = (ErrorResponse<UserDTO>) signUpResult;
                
                return new ErrorResponse<AuthInfoDTO>
                {
                    Message = errorResponse.Message,
                    Code = errorResponse.Code,
                    Errors = errorResponse.Errors,
                    HttpStatusCode = errorResponse.HttpStatusCode
                };
            }
            
            var resultToken = await _tokensService.Authenticate(new CredentialsDTO
            {
                Password = dto.Password,
                Email = signUpResult.Data.Email
            });

            return resultToken;
        }

        // public async Task<Response<AuthInfoDTO>> SignUpMemberGoogle(SignUpMemberGoogleDTO dto)
        // {
        //     var signUpResult = await _service.SignUpMemberGoogle(dto);
        //     if (!signUpResult.IsSuccess)
        //     {
        //         var errorResponse = (ErrorResponse) signUpResult;
        //         
        //         return new ErrorResponse<AuthInfoDTO>
        //         {
        //             Message = errorResponse.Message,
        //             Code = errorResponse.Code,
        //             Errors = errorResponse.Errors,
        //             HttpStatusCode = errorResponse.HttpStatusCode
        //         };
        //     }
        //     
        //     var resultToken = await _tokensService.GoogleAuthenticate(dto.Token);
        //
        //     return resultToken;
        // }

        public async Task<Response<AuthInfoDTO>> SignUp(string email, string password)
        {
            var signUpResult = await _service.SignUp(email, password);
            if (!signUpResult.IsSuccess)
            {
                var errorResponse = (ErrorResponse<UserDTO>) signUpResult;
                
                return new ErrorResponse<AuthInfoDTO>
                {
                    Message = errorResponse.Message,
                    Code = errorResponse.Code,
                    Errors = errorResponse.Errors,
                    HttpStatusCode = errorResponse.HttpStatusCode
                };
            }
            
            var resultToken = await _tokensService.Authenticate(new CredentialsDTO
            {
                Password = password,
                Email = email
            });

            return resultToken;
        }

        // public async Task<Response<AuthInfoDTO>> SignUpGoogle(string googleToken)
        // {
        //     var signUpResult = await _service.SignUpGoogle(googleToken);
        //     if (!signUpResult.IsSuccess)
        //     {
        //         var errorResponse = (ErrorResponse<UserDTO>) signUpResult;
        //         
        //         return new ErrorResponse<AuthInfoDTO>
        //         {
        //             Message = errorResponse.Message,
        //             Code = errorResponse.Code,
        //             Errors = errorResponse.Errors,
        //             HttpStatusCode = errorResponse.HttpStatusCode
        //         };
        //     }
        //     
        //     var resultToken = await _tokensService.GoogleAuthenticate(googleToken);
        //
        //     return resultToken;
        // }

        public async Task<Response<SignUpStepDTO>> SendNewCode(ClaimsPrincipal claims)
        {
            var result = await _service.SendNewCode(claims.GetUserId());
            return result;
        }

        public async Task<Response<SignUpStepDTO>> SignUpConfirm(string code, ClaimsPrincipal claims)
        {
            var result = await _service.SignUpConfirm(claims.GetUserId(), code);
            return result;
        }

        public async Task<Response<SignUpStepDTO<UserDTO>>> SignUpUsername(UserUpdateDTO dto, ClaimsPrincipal claims)
        {
            var result = await _service.SignUpUsername(claims.GetUserId(), dto);
            return result;
        }

        public async Task<Response<SignUpStepDTO<CompanyDTO>>> SignUpCompany(CompanyCreateDTO dto, ClaimsPrincipal claims)
        {
            var result = await _service.SignUpCompany(claims.GetUserId(), dto);
            return result;
        }

        public async Task<Response<SignUpStepDTO<TeamDTO>>> SignUpTeam(TeamCreateDTO dto, ClaimsPrincipal claims)
        {
            var result = await _service.SignUpTeam(claims.GetUserId(), dto);
            return result;
        }

        public async Task<Response<IEnumerable<string>>> GetTeamInvites(ClaimsPrincipal claims)
        {
            var result = await _provider.GetTeamInvites(claims.GetUserId());
            return result;
        }

        public async Task<Response<SignUpStepDTO<ProjectDTO>>> SignUpProject(ProjectCreateDTO dto, ClaimsPrincipal claims)
        {
            var result = await _service.SignUpProject(claims.GetUserId(), dto);
            return result;
        }

        public async Task<Response<SignUpStepDTO>> SignUpDone(ClaimsPrincipal claims)
        {
            var result = await _service.SignUpDone(claims.GetUserId());
            return result;
        }
    }
}