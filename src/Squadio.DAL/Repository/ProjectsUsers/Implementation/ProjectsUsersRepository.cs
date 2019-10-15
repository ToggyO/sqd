using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Projects;

namespace Squadio.DAL.Repository.ProjectsUsers.Implementation
{
    public class ProjectsUsersRepository : IProjectsUsersRepository
    {
        private readonly SquadioDbContext _context;
        public ProjectsUsersRepository(SquadioDbContext context)
        {
            _context = context;
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