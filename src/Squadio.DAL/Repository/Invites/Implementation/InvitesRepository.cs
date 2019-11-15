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
        
        public async Task<InviteModel> CreateInvite(InviteModel entity)
        {
            /*
            var code = Guid.NewGuid().ToString("N");
            var item = new InviteModel
            {
                Email = email,
                Activated = false,
                CreatedDate = DateTime.UtcNow,
                Code = code,
                EntityId = entityId,
                EntityType = entityType
            };
            */
            _context.Invites.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<InviteModel> GetInviteByCode(string code)
        {
            var item = await _context.Invites
                .FirstOrDefaultAsync(x => x.Code.ToUpper() == code.ToUpper());
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

        public async Task ActivateInvites(Guid entityId, IEnumerable<string> emails)
        {
            var query = _context.Invites
                .Where(x => x.EntityId == entityId && x.Activated == false)
                .Where(x => emails.Any(y => y.ToUpper() == x.Email.ToUpper()));
            await query.ForEachAsync(x =>
            {
                x.Activated = true;
                x.ActivatedDate = DateTime.UtcNow;
            });
            _context.Update(query);
            await _context.SaveChangesAsync();
        }

        public async Task<InviteModel> ActivateInvite(string code)
        {
            var item = await GetInviteByCode(code);
            return await ActivateInvite(item.Id);
        }

        public async Task<IEnumerable<InviteModel>> GetInvites(Guid entityId)
        {
            var items = await _context.Invites
                .Where(x => x.EntityId == entityId)
                .ToListAsync();
            return items;
        }
    }
}