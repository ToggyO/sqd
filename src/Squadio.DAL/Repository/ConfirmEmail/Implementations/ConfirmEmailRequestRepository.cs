using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL.Repository.ConfirmEmail.Implementations
{
    public class ConfirmEmailRequestRepository : IConfirmEmailRequestRepository
    {
        private readonly SquadioDbContext _context;
        public ConfirmEmailRequestRepository(SquadioDbContext context)
        {
            _context = context;
        }
        
        public async Task<UserConfirmEmailRequestModel> AddRequest(Guid userId, string code)
        {
            var newRequest = new UserConfirmEmailRequestModel
            {
                UserId = userId,
                Code = code,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            };
            await _context.UserConfirmEmailRequests.AddAsync(newRequest);
            await _context.SaveChangesAsync();
            return newRequest;
        }

        public async Task<UserConfirmEmailRequestModel> GetRequest(Guid userId, string code)
        {
            var item = await _context.UserConfirmEmailRequests
                .Where(x => x.Code == code && x.UserId == userId)
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefaultAsync();
            return item;
        }

        public async Task<UserConfirmEmailRequestModel> GetRequest(string email, string code)
        {
            var item = await _context.UserConfirmEmailRequests
                .Include(x => x.User)
                .Where(x => x.Code == code && x.User.Email.ToUpper() == email.ToUpper())
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefaultAsync();
            return item;
        }
        
        public async Task ActivateAllRequestsForUser(string userEmail)
        {
            var user = await _context.Users
                .Where(x=>x.Email.ToUpper() == userEmail.ToUpper())
                .Where(x => !x.IsDeleted)
                .FirstOrDefaultAsync();
            
            await ActivateAllRequestsForUser(user.Id);
        }

        public async Task ActivateAllRequestsForUser(Guid userId)
        {
            var items = _context.UserConfirmEmailRequests
                .Where(x => x.UserId == userId
                            && !x.IsDeleted);
            
            await items.ForEachAsync(x =>
            {
                x.UpdatedDate = DateTime.UtcNow;
                x.IsDeleted = true;
            });
            
            _context.UserConfirmEmailRequests.UpdateRange(items);
            await _context.SaveChangesAsync();
        }
    }
}