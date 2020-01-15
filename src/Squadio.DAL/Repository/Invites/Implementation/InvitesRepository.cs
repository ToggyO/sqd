﻿using System;
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
            item.IsActivated = true;
            item.ActivatedDate = DateTime.UtcNow;
            _context.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task DeleteInvites(IEnumerable<Guid> ids)
        {
            var query = _context.Invites.Where(x => ids.Contains(x.Id));
            var items = query.ToList();
            _context.Invites.RemoveRange(query);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<InviteModel>> GetInvites(
            Guid? entityId = null, 
            Guid? authorId = null, 
            InviteEntityType? entityType = null, 
            bool? activated = null,
            bool? isSent = null)
        {
            var query = _context.Invites as IQueryable<InviteModel>;
            
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
                query = query.Where(x => x.InviteEntityType == entityType);
            }
            
            if (activated.HasValue)
            {
                query = query.Where(x => x.IsActivated == activated);
            }
            
            if (isSent.HasValue)
            {
                query = query.Where(x => x.IsSent == isSent);
            }
            
            var items = await query
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
            return items;
        }

        public async Task ActivateInvites(Guid entityId, IEnumerable<string> emails)
        {
            var emailsUpper = emails.Select(s => s.ToUpper());

            var query = _context.Invites.Where(x => x.EntityId == entityId && x.IsActivated == false);
            query = query.Where(model => emailsUpper.Contains(model.Email.ToUpper()));
            
            await query.ForEachAsync(x =>
            {
                x.IsActivated = true;
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
    }
}