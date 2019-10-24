using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Invites;

namespace Squadio.DAL.Repository.Invites.Implementation
{
    public class InvitesRepository : IInvitesRepository
    {
        private readonly SquadioDbContext _context;
        public InvitesRepository(SquadioDbContext context)
        {
            _context = context;
        }

        public async Task<InviteModel> CreateTeamInvite(string email, Guid teamId)
        {
            var code = Guid.NewGuid().ToString("N");
            var item = new InviteModel
            {
                Email = email,
                Activated = false,
                CreatedDate = DateTime.UtcNow,
                Code = code,
                EntityId = teamId,
                EntityType = EntityType.Team
            };
            _context.Invites.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<InviteModel> CreateProjectInvite(string email, Guid projectId)
        {
            var code = Guid.NewGuid().ToString("N");
            var item = new InviteModel
            {
                Email = email,
                Activated = false,
                CreatedDate = DateTime.UtcNow,
                Code = code,
                EntityId = projectId,
                EntityType = EntityType.Project
            };
            _context.Invites.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<InviteModel> GetInviteByCode(string code)
        {
            var item = await _context.Invites
                .Where(x => x.Code.ToUpper() == code.ToUpper())
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefaultAsync();
            return item;
        }

        public async Task<InviteModel> ActivateInvite(Guid inviteId)
        {
            var item = await _context.Invites.FindAsync(inviteId);
            item.Activated = true;
            _context.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<IEnumerable<InviteModel>> GetTeamInvites(Guid teamId)
        {
            var items = await _context.Invites
                .Where(x => x.EntityId == teamId 
                            && x.EntityType == EntityType.Team)
                .ToListAsync();
            return items;
        }

        public async Task<IEnumerable<InviteModel>> GetProjectInvites(Guid projectId)
        {
            var items = await _context.Invites
                .Where(x => x.EntityId == projectId 
                            && x.EntityType == EntityType.Project)
                .ToListAsync();
            return items;
        }
    }
}