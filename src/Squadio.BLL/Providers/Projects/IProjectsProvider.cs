﻿using System;
using System.Threading.Tasks;
using Squadio.DTO.Projects;

namespace Squadio.BLL.Providers.Projects
{
    public interface IProjectsProvider
    {
        Task<ProjectDTO> GetById(Guid id);
    }
}