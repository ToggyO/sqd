﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squadio.Common.Models.Pages;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Teams;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL.Repository.TeamsUsers
{
    public interface ITeamsUsersRepository
    {
        Task<PageModel<TeamUserModel>> GetTeamsUsers(PageModel model
            , Guid? userId = null
            , Guid? companyId = null
            , Guid? teamId = null
            , IEnumerable<MembershipStatus> statuses = null);
        // Task<PageModel<TeamUserModel>> GetTeamUsersByEmails(PageModel model, Guid teamId, IEnumerable<string> emails);
        Task<TeamUserModel> GetTeamUserByEmails(Guid teamId, string email);
        Task<TeamUserModel> GetTeamUser(Guid teamId, Guid userId);
        Task AddTeamUser(Guid teamId, Guid userId, MembershipStatus membershipStatus);
        Task DeleteTeamUser(Guid teamId, Guid userId);
        Task DeleteTeamUsers(Guid teamId, IEnumerable<string> emails);
        Task AddRangeTeamUser(Guid teamId, IEnumerable<Guid> userIds, MembershipStatus membershipStatus);
        Task ChangeStatusTeamUser(Guid teamId, Guid userId, MembershipStatus newMembershipStatus);
    }
}