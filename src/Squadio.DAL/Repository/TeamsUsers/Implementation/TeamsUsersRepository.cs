using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Squadio.Common.Models.Pages;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Teams;

namespace Squadio.DAL.Repository.TeamsUsers.Implementation
{
    public class TeamsUsersRepository : ITeamsUsersRepository
    {
        private readonly SquadioDbContext _context;
        public TeamsUsersRepository(SquadioDbContext context)
        {
            _context = context;
        }

        public async Task<PageModel<TeamUserModel>> GetTeamsUsers(PageModel model
            , Guid? userId = null
            , Guid? companyId = null
            , Guid? teamId = null
            , IEnumerable<MembershipStatus> statuses = null)
        {
            IQueryable<TeamUserModel> query = _context.TeamsUsers
                .Include(x => x.User).ThenInclude(x => x.Avatar)
                .Include(x => x.Team).ThenInclude(x=>x.Company);
            
            if (userId.HasValue)
            {
                query = query.Where(x => x.UserId == userId);
            }

            if (companyId.HasValue)
            {
                query = query.Where(x => x.Team.CompanyId == companyId);
            }

            if (teamId.HasValue)
            {
                query = query.Where(x => x.TeamId == teamId);
            }
            
            if (statuses != null)
            {
                var userStatuses = statuses.ToList();
                if (userStatuses?.Any() == true)
                {
                    query = query.Where(x => userStatuses.Contains(x.Status));
                }
            }

            query = query.OrderByDescending(x => x.CreatedDate);
            
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

        public async Task<TeamUserModel> GetTeamUserByEmails(Guid teamId, string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;
            
            var query = _context.TeamsUsers
                .Include(x => x.User).ThenInclude(x => x.Avatar)
                .Include(x => x.Team).ThenInclude(x => x.Company)
                .Where(x => x.TeamId == teamId);

            return await query.FirstOrDefaultAsync(x=>x.User.Email.ToUpper() == email.ToUpper());
        }

        public async Task<PageModel<TeamUserModel>> GetTeamUsersByEmails(PageModel model, Guid teamId, IEnumerable<string> emails)
        {
            
            IQueryable<TeamUserModel> query = _context.TeamsUsers
                .Include(x => x.User).ThenInclude(x => x.Avatar)
                .Include(x => x.Team)
                .Where(x => x.TeamId == teamId);
            
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

            return new PageModel<TeamUserModel>
            {
                Page = model.Page,
                PageSize = model.PageSize,
                Total = total,
                Items = items
            };
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

        public async Task AddTeamUser(Guid teamId, Guid userId, MembershipStatus membershipStatus)
        {
            var item = new TeamUserModel
            {
                TeamId = teamId,
                UserId = userId,
                Status = membershipStatus,
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

        public async Task AddRangeTeamUser(Guid teamId, IEnumerable<Guid> userIds, MembershipStatus membershipStatus)
        {
            var items = userIds.Select(userId => new TeamUserModel
                {
                    TeamId = teamId, 
                    UserId = userId, 
                    Status = membershipStatus, 
                    CreatedDate = DateTime.UtcNow
                })
                .ToList();

            _context.TeamsUsers.AddRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task ChangeStatusTeamUser(Guid teamId, Guid userId, MembershipStatus newMembershipStatus)
        {
            var item = await _context.TeamsUsers
                .Where(x => x.TeamId == teamId && x.UserId == userId)
                .FirstOrDefaultAsync();
            item.Status = newMembershipStatus;
            _context.TeamsUsers.Update(item);
            await _context.SaveChangesAsync();
        }
    }
}