using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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

        public Task<UserModel> Create(UserModel entity)
        {
            throw new NotImplementedException();
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
    }
}
