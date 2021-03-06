using System;
using System.Threading.Tasks;
using AutoMapper;
using Squadio.BLL.Services.Notifications.Emails;
using Squadio.Common.Helpers;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.ConfirmEmail;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Models.Users.Settings;

namespace Squadio.BLL.Services.ConfirmEmail.Implementations
{
    public class ConfirmEmailService : IConfirmEmailService
    {
        private readonly IConfirmEmailRequestRepository _repository;
        private readonly IEmailNotificationsService _emailNotificationsService;
        private readonly IMapper _mapper;

        public ConfirmEmailService(IConfirmEmailRequestRepository repository
            , IEmailNotificationsService emailNotificationsService
            , IMapper mapper)
        {
            _repository = repository;
            _emailNotificationsService = emailNotificationsService;
            _mapper = mapper;
        }

        public async Task<Response<UserConfirmEmailRequestDTO>> AddRequest(Guid userId, string email)
        {
            await _repository.ActivateAllRequestsForUser(userId);
            
            var code = CodeHelper.GenerateNumberCode();
            
            var entity = await _repository.AddRequest(userId, code);

            await _emailNotificationsService.SendConfirmNewMailboxEmail(email, code);
            
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