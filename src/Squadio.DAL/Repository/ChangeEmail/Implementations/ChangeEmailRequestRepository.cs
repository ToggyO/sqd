using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL.Repository.ChangeEmail.Implementation
{
    public class ChangeEmailRequestRepository : IChangeEmailRequestRepository
    {
        private readonly SquadioDbContext _context;
        public ChangeEmailRequestRepository(SquadioDbContext context)
        {
            _context = context;
        }
        
        public async Task<UserChangeEmailRequestModel> AddRequest(Guid userId, string code, string newEmail)
        {
            var newRequest = new UserChangeEmailRequestModel
            {
                UserId = userId,
                Code = code,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false,
                NewEmail = newEmail
            };
            await _context.UserChangeEmailRequests.AddAsync(newRequest);
            await _context.SaveChangesAsync();
            return newRequest;
        }

        public async Task<UserChangeEmailRequestModel> GetRequest(Guid userId, string code)
        {
            var item = await _context.UserChangeEmailRequests
                .Where(x => x.Code == code && x.UserId == userId)
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefaultAsync();
            return item;
        }

        public async Task<UserChangeEmailRequestModel> GetRequest(string email, string code)
        {
            var item = await _context.UserChangeEmailRequests
                .Include(x => x.User)
                .Where(x => x.Code == code && x.User.Email.ToUpper() == email.ToUpper())
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefaultAsync();
            return item;
        }
        
        public async Task ActivateAllRequestsForUser(string userEmail)
        {
            var user = await _context.Users
                .Where(x=>x.Email.ToUpper() == userEmail.ToUpper())
                .FirstOrDefaultAsync();
            
            await ActivateAllRequestsForUser(user.Id);
        }

        public async Task ActivateAllRequestsForUser(Guid userId)
        {
            var items = _context.UserChangeEmailRequests
                .Where(x => x.UserId == userId
                            && !x.IsDeleted);
            
            await items.ForEachAsync(x =>
            {
                x.UpdatedDate = DateTime.UtcNow;
                x.IsDeleted = true;
            });
            
            _context.UserChangeEmailRequests.UpdateRange(items);
            await _context.SaveChangesAsync();
        }
    }
}