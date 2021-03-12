﻿using System;
 using System.Collections.Generic;
 using System.Linq;
using System.Threading.Tasks;
 using AutoMapper;
 using Squadio.BLL.Services.Notifications.Emails;
using Squadio.Common.Helpers;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Companies;
using Squadio.DAL.Repository.Invites;
using Squadio.DAL.Repository.Projects;
using Squadio.DAL.Repository.Teams;
using Squadio.DAL.Repository.Users;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Invites;
 using Squadio.DTO.Models.Invites;

 namespace Squadio.BLL.Services.Invites.Implementations
{
    public class InvitesService : IInvitesService
    {
        private readonly IInvitesRepository _repository;
        private readonly IEmailNotificationsService _emailNotificationsService;
        private readonly IMapper _mapper;

        public InvitesService(IInvitesRepository repository
            , IEmailNotificationsService emailNotificationsService
            , IMapper mapper)
        {
            _repository = repository;
            _emailNotificationsService = emailNotificationsService;
            _mapper = mapper;
        }

        public async Task<Response<InviteDTO>> GetInviteByCode(string code)
        {
            var invites = await _repository.GetInviteByCode(code);
            var mapped = _mapper.Map<InviteDTO>(invites);
            return new Response<InviteDTO>
            {
                Data = mapped
            };
        }

        public async Task<Response<IEnumerable<InviteDTO>>> GetInvites(
            IEnumerable<string> emails = null,
            Guid? entityId = null, 
            InviteEntityType? inviteEntityType = null, 
            Guid? authorId = null)
        {
            var invites = await _repository.GetInvites(entityId, inviteEntityType, emails, authorId);
            var mapped = _mapper.Map<IEnumerable<InviteDTO>>(invites);
            return new Response<IEnumerable<InviteDTO>>
            {
                Data = mapped
            };
        }

        public async Task<Response<InviteDTO>> CreateInvite(string email, Guid entityId, InviteEntityType inviteEntityType, Guid authorId, string code = null)
        {
            if (string.IsNullOrEmpty(code))
            {
                code = CodeHelper.GenerateCodeAsGuid();
            }
            
            var invite = new InviteModel
            {
                Email = email,
                IsDeleted = false,
                IsSent = false,
                CreatedDate = DateTime.UtcNow,
                Code = code,
                EntityId = entityId,
                EntityType = inviteEntityType,
                CreatorId = authorId
            };
            
            invite = await _repository.CreateInvite(invite);

            var mappedInvite = _mapper.Map<InviteDTO>(invite);
            
            return new Response<InviteDTO>
            {
                Data = mappedInvite
            };
        }

        public async Task<Response> SendInvite(string email, Guid entityId, InviteEntityType inviteEntityType)
        {
            var invites = (await _repository.GetInvites(entityId, inviteEntityType, new []{email})).ToList();
            var mainInvite = invites.FirstOrDefault();
            if (mainInvite != null)
            {
                await _emailNotificationsService.SendInviteEmail(
                    email,
                    mainInvite.Creator.Name,
                    //TODO
                    "GET NAME LATER",
                    inviteEntityType.ToString().ToLower(),
                    mainInvite.Code);
            }

            invites.Remove(mainInvite);
            await _repository.DeleteInvites(invites.Select(x => x.Id));
            return new Response();
        }

        public async Task<Response> RemoveInvite(string email, Guid entityId, InviteEntityType inviteEntityType)
        {
            var invites = await _repository.GetInvites(entityId, inviteEntityType, new []{email});
            await _repository.DeleteInvites(invites.Select(x => x.Id));
            return new Response();
        }

        public async Task<Response> ActivateInvite(string email, Guid entityId)
        {
            await _repository.ActivateInvites(entityId, email);
            return new Response();
        }
    }
}