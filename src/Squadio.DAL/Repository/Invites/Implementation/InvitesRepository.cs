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
            item.ActivatedDate = DateTime.UtcNow;
            _context.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task ActivateInvites(Guid entityId, IEnumerable<string> emails)
        {
            var emailsUpper = emails.Select(s => s.ToUpper());

            var query = _context.Invites.Where(x => x.EntityId == entityId && x.Activated == false);
            query = query.Where(model => emailsUpper.Contains(model.Email.ToUpper()));
            
            await query.ForEachAsync(x =>
            {
                x.Activated = true;
                x.ActivatedDate = DateTime.UtcNow;
            });
            _context.UpdateRange(query);
            await _context.SaveChangesAsync();
        }

        public async Task<InviteModel> ActivateInvite(string code)
        {
            var item = await GetInviteByCode(code);
            return await ActivateInvite(item.Id);
        }

        public async Task<IEnumerable<InviteModel>> GetInvites(Guid entityId, bool? activated = null)
        {
            var query = _context.Invites
                .Where(x => x.EntityId == entityId);
            
            if (activated.HasValue)
            {
                query = query.Where(x => x.Activated == activated);
            }
            
            var items = await query
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
            return items;
        }
    }
}