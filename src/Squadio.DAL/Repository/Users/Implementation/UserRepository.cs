using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL.Repository.Users.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly SquadioDbContext _context;
        public UserRepository(SquadioDbContext context)
        {
            _context = context;
        }

        public async Task<UserModel> Create(UserModel entity)
        {
            var user = await _context.Users.AddAsync(entity);
            await _context.SaveChangesAsync();
            var result = user.Entity;
            return result;
        }

        public Task<UserModel> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<UserModel> GetById(Guid id)
        {
            var entity = await _context.Users.FindAsync(id);
            return entity;
        }

        public Task<UserModel> Update(UserModel entity)
        {
            throw new NotImplementedException();
        }

        public async Task<UserModel> GetByEmail(string email)
        {
            var entity = await _context.Users
                .Where(x => string.Equals(x.Email, email, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefaultAsync();
            return entity;
        }

        public async Task AddPasswordRequest(Guid userId, string code)
        {
            var user = await GetById(userId);
            var newRequest = new UserPasswordRequestModel
            {
                UserId = user.Id,
                Code = code,
                CreatedDate = DateTime.UtcNow
            };
            await _context.UserPasswordRequests.AddAsync(newRequest);
            await _context.SaveChangesAsync();
        }
    }
}
