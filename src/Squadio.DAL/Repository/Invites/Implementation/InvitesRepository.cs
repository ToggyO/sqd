using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task<InviteModel> CreateInvite(string email)
        {
            var code = Guid.NewGuid().ToString("N");
            var item = new InviteModel
            {
                Email = email,
                Activated = false,
                CreatedDate = DateTime.UtcNow,
                Code = code
            };
            _context.Invites.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<InviteModel> GetInviteByEmail(string email)
        {
            var item = await _context.Invites
                .Where(x => x.Email.ToUpper() == email.ToUpper())
                .FirstOrDefaultAsync();
            return item;
        }

        public async Task<InviteModel> GetInviteByCode(string code)
        {
            var item = await _context.Invites
                .Where(x => x.Code.ToUpper() == code.ToUpper())
                .FirstOrDefaultAsync();
            return item;
        }

        public async Task<InviteModel> AcceptInvite(Guid inviteId)
        {
            var item = await _context.Invites.FindAsync(inviteId);
            item.Activated = true;
            _context.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }
    }
}