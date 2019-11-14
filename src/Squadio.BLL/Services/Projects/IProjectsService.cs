﻿using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Projects;

namespace Squadio.BLL.Services.Projects
{
    public interface IProjectsService
    {
        Task<Response<ProjectDTO>> Create(Guid userId, Guid companyId, CreateProjectDTO dto);
        Task<Response<ProjectDTO>> Update(Guid projectId, Guid userId, ProjectUpdateDTO dto);
        Task<Response> DeleteUserFromProject(Guid projectId, Guid removeUserId, Guid currentUserId);
    }
}