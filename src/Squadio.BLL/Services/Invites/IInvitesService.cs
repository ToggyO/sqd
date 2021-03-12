﻿using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.Domain.Enums;
using Squadio.DTO.Models.Invites;

namespace Squadio.BLL.Services.Invites
{
    public interface IInvitesService
    {
        Task<Response> GetInviteByCode(string code);
        Task<Response> CreateInvite(string email, Guid entityId, InviteEntityType inviteEntityType, Guid authorId, string code = null);
        Task<Response> SendInvite(string email);
        Task<Response> RemoveInvites(string email, Guid entityId, InviteEntityType inviteEntityType);
    }
}