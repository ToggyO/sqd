using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Projects;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Pages;

namespace Squadio.DAL.Repository.ProjectsUsers.Implementation
{
    public class ProjectsUsersRepository : IProjectsUsersRepository
    {
        private readonly SquadioDbContext _context;
        public ProjectsUsersRepository(SquadioDbContext context)
        {
            _context = context;
        }

        public async Task<PageModel<UserModel>> GetProjectUsers(Guid projectId, PageModel model)
        {
            var query = _context.ProjectsUsers
                .Include(x => x.User)
                .Where(x => x.ProjectId == projectId);
            
            var total = await query.CountAsync();
            var items = await query
                .Skip((model.Page - 1) * model.PageSize)
                .Take(model.PageSize)
                .Select(x => x.User)
                .ToListAsync();
            
            var result = new PageModel<UserModel>
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