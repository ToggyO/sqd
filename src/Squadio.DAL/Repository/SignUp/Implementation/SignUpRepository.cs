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

        public async Task<UserRegistrationStepModel> SetRegistrationStep(Guid userId, RegistrationStep step, MembershipStatus? status = null)
        {
            var item = await _context.UsersRegistrationStep
                .Where(x => x.UserId == userId)
                .FirstOrDefaultAsync();

            var stepExisted = true;

            if (item == null)
            {
                item = new UserRegistrationStepModel { UserId = userId, CreatedDate = DateTime.Now};
                stepExisted = false;
            }

            item.Step = step;
            item.UpdatedDate = DateTime.UtcNow;

            if (status.HasValue)
            {
                item.Status = status.Value;
            }
            
            if(stepExisted)
                _context.Update(item);
            else 
                _context.UsersRegistrationStep.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }
    }
}