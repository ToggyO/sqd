using System;
using System.Net;
using System.Threading.Tasks;
using Squadio.BLL.Services.Email;
using Squadio.Common.Models.Email;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Invites;
using Squadio.DTO.Invites;

namespace Squadio.BLL.Services.Invites.Implementation
{
    public class InvitesService : IInvitesService
    {
        private readonly IInvitesRepository _repository;
        private readonly IEmailService<InviteToTeamEmailModel> _inviteToTeamMailService;
        private readonly IEmailService<InviteToProjectEmailModel> _inviteToProjectMailService;
        public InvitesService(IInvitesRepository repository
            , IEmailService<InviteToTeamEmailModel> inviteToTeamMailService
            , IEmailService<InviteToProjectEmailModel> inviteToProjectMailService)
        {
            _repository = repository;
            _inviteToTeamMailService = inviteToTeamMailService;
            _inviteToProjectMailService = inviteToProjectMailService;
        }

        public async Task<Response> InviteToTeam(string authorName, string teamName, Guid teamId, string email)
        {
            var invite = await _repository.CreateInvite(email);
            try
            {
                await _inviteToTeamMailService.Send(new InviteToTeamEmailModel
                {
                    To = email,
                    AuthorName = authorName,
                    Code = invite.Code,
                    TeamId = teamId.ToString(),
                    TeamName = teamName
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return new Response();
        }

        public async Task<Response> InviteToProject(string authorName, string projectName, Guid projectId, string email)
        {
            var invite = await _repository.CreateInvite(email);
            try
            {
                await _inviteToProjectMailService.Send(new InviteToProjectEmailModel
                {
                    To = email,
                    AuthorName = authorName,
                    Code = invite.Code,
                    ProjectId = projectId.ToString(),
                    ProjectName = projectName
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return new Response();
        }

        public async Task<Response> AcceptInviteToTeam(Guid userId, Guid teamId, string code)
        {
            var invite = await _repository.GetInviteByCode(code);
            
            if (invite == null || invite?.Activated == true)
            {
                return new ErrorResponse
                {
                    Code = ErrorCodes.Security.InviteInvalid,
                    Message = ErrorMessages.Security.InviteInvalid,
                    HttpStatusCode = HttpStatusCode.BadRequest
                };
            }
            
            // TODO: add user to team  
            
            return new Response();
        }

        public async Task<Response> AcceptInviteToProject(Guid userId, Guid projectId, string code)
        {
            var invite = await _repository.GetInviteByCode(code);
            
            if (invite == null || invite?.Activated == true)
            {
                return new ErrorResponse
                {
                    Code = ErrorCodes.Security.InviteInvalid,
                    Message = ErrorMessages.Security.InviteInvalid,
                    HttpStatusCode = HttpStatusCode.BadRequest
                };
            }
            
            // TODO: add user to project   
            
            return new Response();
        }
    }
}