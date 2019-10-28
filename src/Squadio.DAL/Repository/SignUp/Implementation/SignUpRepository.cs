using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL.Repository.SignUp.Implementation
{
    public class SignUpRepository : ISignUpRepository
    {
        private readonly SquadioDbContext _context;
        public SignUpRepository(SquadioDbContext context)
        {
            _context = context;
        }

        public async Task<UserRegistrationStepModel> GetRegistrationStepByEmail(string email)
        {
            var item = await _context.UsersRegistrationStep
                .Include(x => x.User)
                .Where(x => x.User.Email.ToUpper() == email.ToUpper())
                .FirstOrDefaultAsync();
            return item;
        }

        public async Task<UserRegistrationStepModel> GetRegistrationStepByUserId(Guid userId)
        {
            var item = await _context.UsersRegistrationStep
                .Include(x => x.User)
                .Where(x => x.UserId == userId)
                .FirstOrDefaultAsync();
            return item;
        }

        public async Task<UserRegistrationStepModel> SetRegistrationStep(Guid userId, RegistrationStep step)
        {
            var item = await _context.UsersRegistrationStep
                .Where(x => x.UserId == userId)
                .FirstOrDefaultAsync();

            var stepExisted = true;

            if (item == null)
            {
                item = new UserRegistrationStepModel { UserId = userId };
                stepExisted = false;
            }

            item.Step = step;
            item.UpdatedDate = DateTime.UtcNow;
            
            if(stepExisted)
                _context.Update(item);
            else 
                _context.UsersRegistrationStep.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<UserSignUpRequestModel> AddRequest(Guid userId, string code)
        {
            var newRequest = new UserSignUpRequestModel
            {
                UserId = userId,
                Code = code,
                CreatedDate = DateTime.UtcNow,
                IsActivated = false
            };
            await _context.UserSignUpRequests.AddAsync(newRequest);
            await _context.SaveChangesAsync();
            return newRequest;
        }

        public async Task<UserSignUpRequestModel> GetRequest(Guid userId, string code)
        {
            var item = await _context.UserSignUpRequests
                .Where(x => x.Code == code && x.UserId == userId)
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefaultAsync();
            return item;
        }

        public async Task<UserSignUpRequestModel> GetRequest(string email, string code)
        {
            var item = await _context.UserSignUpRequests
                .Include(x => x.User)
                .Where(x => x.Code == code && x.User.Email.ToUpper() == email.ToUpper())
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefaultAsync();
            return item;
        }

        public async Task<UserSignUpRequestModel> ActivateRequest(Guid requestId)
        {
            var item = await _context.UserSignUpRequests.FindAsync(requestId);
            item.ActivatedDate = DateTime.UtcNow;
            item.IsActivated = true;
            _context.UserSignUpRequests.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task ActivateAllRequestsForUser(Guid userId)
        {
            var items = _context.UserSignUpRequests
                .Where(x => x.UserId == userId
                            && !x.IsActivated);
            
            await items.ForEachAsync(x =>
            {
                x.ActivatedDate = DateTime.UtcNow;
                x.IsActivated = true;
            });
            
            _context.UserSignUpRequests.UpdateRange(items);
            await _context.SaveChangesAsync();
        }
    }
}