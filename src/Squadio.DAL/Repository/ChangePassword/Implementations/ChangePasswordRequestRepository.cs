using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL.Repository.ChangePassword.Implementations
{
    public class ChangePasswordRequestRepository : IChangePasswordRequestRepository
    {
        private readonly SquadioDbContext _context;
        public ChangePasswordRequestRepository(SquadioDbContext context)
        {
            _context = context;
        }

        public async Task<UserRestorePasswordRequestModel> GetRequestByCode(string code)
        {
            var item = await _context.UserRestorePasswordRequests
                .Include(x => x.User)
                .OrderByDescending(x => x.CreatedDate)
                .Where(x => x.Code.ToUpper() == code.ToUpper() &&
                            !x.IsDeleted)
                .FirstOrDefaultAsync();
            return item;
        }

        public async Task<UserRestorePasswordRequestModel> GetRequest(Guid id)
        {
            var item = await _context.UserRestorePasswordRequests
                .Where(x => !x.IsDeleted)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }
        
        public async Task<UserRestorePasswordRequestModel> AddRequest(Guid userId, string code)
        {
            var newRequest = new UserRestorePasswordRequestModel
            {
                UserId = userId,
                Code = code,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            };
            await _context.UserRestorePasswordRequests.AddAsync(newRequest);
            await _context.SaveChangesAsync();
            return newRequest;
        }

        public async Task ActivateAllRequestsForUser(Guid userId)
        {
            var items = _context.UserRestorePasswordRequests
                .Where(x => x.UserId == userId 
                            && !x.IsDeleted);
            
            await items.ForEachAsync(x =>
            {
                x.UpdatedDate = DateTime.UtcNow;
                x.IsDeleted = true;
            });
            
            _context.UserRestorePasswordRequests.UpdateRange(items);
            await _context.SaveChangesAsync();
        }
    }
}