﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Squadio.Common.Models.Pages;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Projects;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL.Repository.ProjectsUsers.Implementation
{
    public class ProjectsUsersRepository : IProjectsUsersRepository
    {
        private readonly SquadioDbContext _context;
        public ProjectsUsersRepository(SquadioDbContext context)
        {
            _context = context;
        }

        public async Task<PageModel<ProjectUserModel>> GetUserProjects(Guid userId, PageModel model, Guid? companyId = null)
        {
            var query = _context.ProjectsUsers
                .Include(x => x.User)
                .Include(x => x.Project)
                .Where(x => x.UserId == userId);

            if (companyId.HasValue)
            {
                query = query.Where(x => x.Project.CompanyId == companyId);
            }
            
            var total = await query.CountAsync();
            var items = await query
                .Skip((model.Page - 1) * model.PageSize)
                .Take(model.PageSize)
                .ToListAsync();
            
            var result = new PageModel<ProjectUserModel>
            {
                Page = model.Page,
                PageSize = model.PageSize,
                Total = total,
                Items = items
            };
            return result;
        }

        public async Task<PageModel<ProjectUserModel>> GetProjectUsers(Guid projectId, PageModel model)
        {
            var query = _context.ProjectsUsers
                .Include(x => x.User)
                .Include(x => x.Project)
                .Where(x => x.ProjectId == projectId);
            
            var total = await query.CountAsync();
            var items = await query
                .Skip((model.Page - 1) * model.PageSize)
                .Take(model.PageSize)
                .ToListAsync();
            
            var result = new PageModel<ProjectUserModel>
            {
                Page = model.Page,
                PageSize = model.PageSize,
                Total = total,
                Items = items
            };
            return result;
        }

        public async Task<ProjectUserModel> GetProjectUser(Guid projectId, Guid userId)
        {
            var item = await _context.ProjectsUsers
                .Include(x => x.Project)
                .Include(x => x.User)
                .Where(x => x.ProjectId == projectId && x.UserId == userId)
                .FirstOrDefaultAsync();
            return item;
        }

        public async Task AddProjectUser(Guid projectId, Guid userId, UserStatus userStatus)
        {
            var item = new ProjectUserModel
            {
                ProjectId = projectId,
                UserId = userId,
                Status = userStatus,
                CreatedDate = DateTime.UtcNow
            };
            _context.ProjectsUsers.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProjectUser(Guid projectId, Guid userId)
        {
            var item = await _context.ProjectsUsers
                .Where(x => x.ProjectId == projectId && x.UserId == userId)
                .FirstOrDefaultAsync();
            _context.ProjectsUsers.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeProjectUser(Guid projectId, IEnumerable<Guid> userIds, UserStatus userStatus)
        {
            var items = userIds.Select(userId => new ProjectUserModel
                {
                    ProjectId = projectId, 
                    UserId = userId, 
                    Status = userStatus, 
                    CreatedDate = DateTime.UtcNow
                })
                .ToList();

            _context.ProjectsUsers.AddRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task ChangeStatusProjectUser(Guid projectId, Guid userId, UserStatus newUserStatus)
        {
            var item = await _context.ProjectsUsers
                .Where(x => x.ProjectId == projectId && x.UserId == userId)
                .FirstOrDefaultAsync();
            item.Status = newUserStatus;
            _context.ProjectsUsers.Update(item);
            await _context.SaveChangesAsync();
        }
    }
}