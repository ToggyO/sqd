using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Squadio.Common.Models.Pages;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Teams;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL.Repository.TeamsUsers.Implementation
{
    public class TeamsUsersRepository : ITeamsUsersRepository
    {
        private readonly SquadioDbContext _context;
        public TeamsUsersRepository(SquadioDbContext context)
        {
            _context = context;
        }

        public async Task<PageModel<TeamUserModel>> GetUserTeams(Guid userId, PageModel model, Guid? companyId = null)
        {
            var query = _context.TeamsUsers
                .Include(x => x.User)
                .Include(x => x.Team)
                .Where(x => x.UserId == userId);

            if (companyId.HasValue)
            {
                query = query.Where(x => x.Team.CompanyId == companyId);
            }
            
            var total = await query.CountAsync();
            var items = await query
                .Skip((model.Page - 1) * model.PageSize)
                .Take(model.PageSize)
                .ToListAsync();
            
            var result = new PageModel<TeamUserModel>
            {
                Page = model.Page,
                PageSize = model.PageSize,
                Total = total,
                Items = items
            };
            return result;
        }

        public async Task<PageModel<TeamUserModel>> GetTeamUsers(Guid teamId, PageModel model)
        {
            var query = _context.TeamsUsers
                .Include(x => x.User)
                .Include(x => x.Team)
                .Where(x => x.TeamId == teamId);
            
            var total = await query.CountAsync();
            var items = await query
                .Skip((model.Page - 1) * model.PageSize)
                .Take(model.PageSize)
                .ToListAsync();
            
            var result = new PageModel<TeamUserModel>
            {
                Page = model.Page,
                PageSize = model.PageSize,
                Total = total,
                Items = items
            };
            return result;
        }

        public async Task<TeamUserModel> GetTeamUser(Guid teamId, Guid userId)
        {
            var item = await _context.TeamsUsers
                .Include(x => x.Team)
                .Include(x => x.User)
                .Where(x => x.TeamId == teamId && x.UserId == userId)
                .FirstOrDefaultAsync();
            return item;
        }

        public async Task<TeamUserModel> GetFullTeamUser(Guid teamId, Guid userId)
        {
            var item = await _context.TeamsUsers
                .Include(x => x.Team)
                    .ThenInclude(x=>x.Company)
                .Include(x => x.User)
                    .ThenInclude(x=>x.Role)
                .Where(x => x.TeamId == teamId && x.UserId == userId)
                .FirstOrDefaultAsync();
            return item;
        }

        public async Task AddTeamUser(Guid teamId, Guid userId, UserStatus userStatus)
        {
            var item = new TeamUserModel
            {
                TeamId = teamId,
                UserId = userId,
                Status = userStatus,
                CreatedDate = DateTime.UtcNow
            };
            _context.TeamsUsers.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTeamUser(Guid teamId, Guid userId)
        {
            var item = await _context.TeamsUsers
                .Where(x => x.TeamId == teamId && x.UserId == userId)
                .FirstOrDefaultAsync();
            _context.TeamsUsers.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTeamUsers(Guid teamId, IEnumerable<string> emails)
        {
            var emailsUpper = emails.Select(s => s.ToUpper());

            var query = _context.TeamsUsers
                .Include(x => x.User)
                .Where(x => x.TeamId == teamId);
            query = query.Where(x => emailsUpper.Contains(x.User.Email.ToUpper()));

            var items = await query.ToListAsync();
            
            _context.TeamsUsers.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeTeamUser(Guid teamId, IEnumerable<Guid> userIds, UserStatus userStatus)
        {
            var items = userIds.Select(userId => new TeamUserModel
                {
                    TeamId = teamId, 
                    UserId = userId, 
                    Status = userStatus, 
                    CreatedDate = DateTime.UtcNow
                })
                .ToList();

            _context.TeamsUsers.AddRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task ChangeStatusTeamUser(Guid teamId, Guid userId, UserStatus newUserStatus)
        {
            var item = await _context.TeamsUsers
                .Where(x => x.TeamId == teamId && x.UserId == userId)
                .FirstOrDefaultAsync();
            item.Status = newUserStatus;
            _context.TeamsUsers.Update(item);
            await _context.SaveChangesAsync();
        }
    }
}