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
            _context.Invites.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<InviteModel> GetInviteByCode(string code)
        {
            var item = await _context.Invites
                .Include(x => x.Creator)
                .FirstOrDefaultAsync(x => x.Code.ToUpper() == code.ToUpper());
            return item;
        }

        public async Task<InviteModel> ActivateInvite(Guid inviteId)
        {
            var item = await _context.Invites.FindAsync(inviteId);
            item.IsDeleted = true;
            item.UpdatedDate = DateTime.UtcNow;
            _context.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<InviteModel> ActivateInvite(string code)
        {
            var item = await GetInviteByCode(code);
            return await ActivateInvite(item.Id);
        }

        public async Task DeleteInvites(IEnumerable<Guid> ids)
        {
            var query = _context.Invites.Where(x => ids.Contains(x.Id));
            await query.ForEachAsync(x =>
            {
                x.IsDeleted = true;
                x.UpdatedDate = DateTime.UtcNow;
            });
            _context.UpdateRange(query);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<InviteModel>> GetInvites(
            Guid? entityId = null, 
            Guid? authorId = null, 
            string email = null, 
            InviteEntityType? entityType = null)
        {
            var query = _context.Invites
                .Include(x => x.Creator) as IQueryable<InviteModel>;
            
            if (entityId.HasValue)
            {
                query = query.Where(x => x.EntityId == entityId);
            }
            
            if (authorId.HasValue)
            {
                query = query.Where(x => x.CreatorId == authorId);
            }
            
            if (entityType.HasValue)
            {
                query = query.Where(x => x.EntityType == entityType);
            }
            
            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(x => x.Email.ToUpper() == email.ToUpper());
            }
            
            query = query.Where(x => x.IsDeleted != true);
            
            var items = await query
                .ToListAsync();
            return items;
        }

        public async Task ActivateInvites(Guid entityId, string email)
        {
            if(string.IsNullOrEmpty(email))
                return;

            var query = _context.Invites.Where(x => x.EntityId == entityId && x.IsDeleted == false);
            query = query.Where(x => x.Email.ToUpper() == email.ToUpper());
            
            await query.ForEachAsync(x =>
            {
                x.IsDeleted = true;
                x.UpdatedDate = DateTime.UtcNow;
            });
            _context.UpdateRange(query);
            await _context.SaveChangesAsync();
        }
    }
}