﻿using System;
 using System.Collections.Generic;
 using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.Domain.Enums;
using Squadio.DTO.Models.Invites;

namespace Squadio.BLL.Services.Invites
{
    public interface IInvitesService
    {
        Task<Response<InviteDTO>> GetInviteByCode(string code);
        Task<Response<IEnumerable<InviteDTO>>> GetInvites(IEnumerable<string> emails, Guid entityId, InviteEntityType inviteEntityType);
        Task<Response<InviteDTO>> CreateInvite(string email, Guid entityId, InviteEntityType inviteEntityType, Guid authorId, string code = null);
        Task<Response> SendInvite(string email, Guid entityId, InviteEntityType inviteEntityType);
        Task<Response> RemoveInvite(string email, Guid entityId, InviteEntityType inviteEntityType);
    }
}