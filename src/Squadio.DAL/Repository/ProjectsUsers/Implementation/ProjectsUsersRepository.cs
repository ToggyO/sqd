using System;
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

        public async Task<PageModel<ProjectUserModel>> GetProjectsUsers(PageModel model
            , Guid? userId = null
            , Guid? companyId = null
            , Guid? teamId = null
            , Guid? projectId = null
            , IEnumerable<MembershipStatus> statuses = null)
        {
            IQueryable<ProjectUserModel> query = _context.ProjectsUsers
                    .Include(x => x.User).ThenInclude(x => x.Avatar)
                    .Include(x => x.Project).ThenInclude(x=>x.Team).ThenInclude(x=>x.Company);

            if (userId.HasValue)
            {
                query = query.Where(x => x.UserId == userId);
            }
            
            if (companyId.HasValue)
            {
                query = query.Where(x => x.Project.Team.CompanyId == companyId);
            }

            if (teamId.HasValue)
            {
                query = query.Where(x => x.Project.TeamId == teamId);
            }

            if (projectId.HasValue)
            {
                query = query.Where(x => x.ProjectId == projectId);
            }
            
            if (statuses != null)
            {
                var userStatuses = statuses.ToList();
                if (userStatuses?.Any() == true)
                {
                    query = query.Where(x => userStatuses.Contains(x.Status));
                }
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

        public async Task<ProjectUserModel> GetProjectUserByEmail(Guid projectId, string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;

            var query = _context.ProjectsUsers
                .Include(x => x.User).ThenInclude(x => x.Avatar)
                .Include(x => x.Project).ThenInclude(x => x.Team).ThenInclude(x => x.Company)
                .Where(x => x.ProjectId == projectId);

            return await query.FirstOrDefaultAsync(x=>x.User.Email.ToUpper() == email.ToUpper());
        }

        public async Task<PageModel<ProjectUserModel>> GetProjectUsersByEmails(PageModel model, Guid projectId, IEnumerable<string> emails)
        {
            
            IQueryable<ProjectUserModel> query = _context.ProjectsUsers
                .Include(x => x.User).ThenInclude(x => x.Avatar)
                .Include(x => x.Project)
                .Where(x => x.ProjectId == projectId);
            
            if (emails != null)
            {
                var userEmails = emails.ToList();
                if (userEmails?.Any() == true)
                {
                    query = query.Where(x => userEmails.Contains(x.User.Email));
                }
            }
            
            query = query.OrderBy(x => x.User.Email);
            
            var skip = (model.Page - 1) * model.PageSize;
            var take = model.PageSize;

            var total = await query.CountAsync();
            var items = await query
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return new PageModel<ProjectUserModel>
            {
                Page = model.Page,
                PageSize = model.PageSize,
                Total = total,
                Items = items
            };
        }

        public async Task<ProjectUserModel> GetProjectUser(Guid projectId, Guid userId)
        {
            var item = await _context.ProjectsUsers
                .Where(x => x.ProjectId == projectId && x.UserId == userId)
                .Include(x => x.Project)
                .Include(x => x.User)
                .FirstOrDefaultAsync();
            return item;
        }

        public async Task<ProjectUserModel> GetFullProjectUser(Guid projectId, Guid userId)
        {
            var item = await _context.ProjectsUsers
                .Where(x => x.ProjectId == projectId && x.UserId == userId)
                .Include(x => x.Project).ThenInclude(x=>x.Team).ThenInclude(x=>x.Company)
                .Include(x => x.User).ThenInclude(x=>x.Avatar)
                .FirstOrDefaultAsync();
            return item;
        }

        public async Task AddProjectUser(Guid projectId, Guid userId, MembershipStatus membershipStatus)
        {
            var item = new ProjectUserModel
            {
                ProjectId = projectId,
                UserId = userId,
                Status = membershipStatus,
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

        public async Task DeleteProjectUsers(Guid projectId, IEnumerable<string> emails)
        {
            var emailsUpper = emails.Select(s => s.ToUpper());

            var query = _context.ProjectsUsers
                .Include(x => x.User)
                .Where(x => x.ProjectId == projectId);
            query = query.Where(x => emailsUpper.Contains(x.User.Email.ToUpper()));

            var items = await query.ToListAsync();
            
            _context.ProjectsUsers.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeProjectUser(Guid projectId, IEnumerable<Guid> userIds, MembershipStatus membershipStatus)
        {
            var items = userIds.Select(userId => new ProjectUserModel
                {
                    ProjectId = projectId, 
                    UserId = userId, 
                    Status = membershipStatus, 
                    CreatedDate = DateTime.UtcNow
                })
                .ToList();

            _context.ProjectsUsers.AddRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task ChangeStatusProjectUser(Guid projectId, Guid userId, MembershipStatus newMembershipStatus)
        {
            var item = await _context.ProjectsUsers
                .Where(x => x.ProjectId == projectId && x.UserId == userId)
                .FirstOrDefaultAsync();
            item.Status = newMembershipStatus;
            _context.ProjectsUsers.Update(item);
            await _context.SaveChangesAsync();
        }
    }
}