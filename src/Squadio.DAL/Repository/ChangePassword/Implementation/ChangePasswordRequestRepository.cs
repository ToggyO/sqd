using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL.Repository.ChangePassword.Implementation
{
    public class ChangePasswordRequestRepository : IChangePasswordRequestRepository
    {
        private readonly SquadioDbContext _context;
        public ChangePasswordRequestRepository(SquadioDbContext context)
        {
            _context = context;
        }

        public async Task<UserPasswordRequestModel> GetRequestByCode(string code)
        {
            var item = await _context.UserPasswordRequests
                .Include(x => x.User)
                .OrderByDescending(x => x.CreatedDate)
                .Where(x => x.Code.ToUpper() == code.ToUpper() &&
                            x.IsActivated == false)
                .FirstOrDefaultAsync();
            return item;
        }

        public async Task<UserPasswordRequestModel> GetRequest(Guid id)
        {
            var item = await _context.UserPasswordRequests.FindAsync(id);
            return item;
        }
        
        public async Task<UserPasswordRequestModel> AddRequest(Guid userId, string code)
        {
            var newRequest = new UserPasswordRequestModel
            {
                UserId = userId,
                Code = code,
                CreatedDate = DateTime.UtcNow,
                IsActivated = false
            };
            await _context.UserPasswordRequests.AddAsync(newRequest);
            await _context.SaveChangesAsync();
            return newRequest;
        }

        public async Task ActivateAllRequestsForUser(Guid userId)
        {
            var items = _context.UserPasswordRequests
                .Where(x => x.UserId == userId 
                            && x.IsActivated == false);
            
            await items.ForEachAsync(x =>
            {
                x.ActivatedDate = DateTime.UtcNow;
                x.IsActivated = true;
            });
            
            _context.UserPasswordRequests.UpdateRange(items);
            await _context.SaveChangesAsync();
        }
    }
}