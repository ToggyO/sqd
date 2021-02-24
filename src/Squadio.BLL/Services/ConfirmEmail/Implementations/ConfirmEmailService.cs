using System;
using System.Threading.Tasks;
using AutoMapper;
using Squadio.BLL.Providers.Codes;
using Squadio.BLL.Services.Rabbit;
using Squadio.Common.Models.Email;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.ConfirmEmail;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Users.Settings;

namespace Squadio.BLL.Services.ConfirmEmail.Implementations
{
    public class ConfirmEmailService : IConfirmEmailService
    {
        private readonly IConfirmEmailRequestRepository _repository;
        private readonly ICodeProvider _codeProvider;
        private readonly IRabbitService _rabbitService;
        private readonly IMapper _mapper;

        public ConfirmEmailService(IConfirmEmailRequestRepository repository
            , IRabbitService rabbitService
            , ICodeProvider codeProvider
            , IMapper mapper)
        {
            _repository = repository;
            _rabbitService = rabbitService;
            _codeProvider = codeProvider;
            _mapper = mapper;
        }

        public async Task<Response<UserConfirmEmailRequestDTO>> AddRequest(Guid userId, string email)
        {
            await _repository.ActivateAllRequestsForUser(userId);

            
            var code = _codeProvider.GenerateNumberCode();
            
            await _rabbitService.Send(new UserConfirmEmailModel()
            {
                Code = code,
                To = email
            });
            
            var entity = await _repository.AddRequest(userId, code);
            
            return new Response<UserConfirmEmailRequestDTO>
            {
                Data = _mapper.Map<UserConfirmEmailRequestModel, UserConfirmEmailRequestDTO>(entity)
            };
        }

        public async Task<Response<UserConfirmEmailRequestDTO>> GetRequest(Guid userId, string code)
        {
            var entity = await _repository.GetRequest(userId, code);

            if (entity == null)
            {
                return new BusinessConflictErrorResponse<UserConfirmEmailRequestDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.ConfirmationCodeInvalid,
                        Message = ErrorMessages.Security.ConfirmationCodeInvalid
                    }
                });
            }
            
            return new Response<UserConfirmEmailRequestDTO>
            {
                Data = _mapper.Map<UserConfirmEmailRequestModel, UserConfirmEmailRequestDTO>(entity)
            };
        }

        public async Task<Response> ActivateAllRequests(Guid userId)
        {
            await _repository.ActivateAllRequestsForUser(userId);
            return new Response();
        }
    }
}