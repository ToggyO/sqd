﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squadio.Domain.Enums;

namespace Squadio.DAL.Repository.ProjectsUsers
{
    public interface IProjectsUsersRepository
    {
        Task AddProjectUser(Guid projectId, Guid userId, UserStatus userStatus);
        Task AddRangeProjectUser(Guid projectId, IEnumerable<Guid> userIds, UserStatus userStatus);
        Task ChangeStatusProjectUser(Guid projectId, Guid userId, UserStatus newUserStatus);
    }
}