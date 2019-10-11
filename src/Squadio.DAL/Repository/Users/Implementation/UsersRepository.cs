using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL.Repository.Users.Implementation
{
    public class UsersRepository : IUsersRepository
    {
        private readonly SquadioDbContext _context;
        public UsersRepository(SquadioDbContext context)
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

        public async Task<UserModel> Update(UserModel userUpdateModel)
        {
            var entity = await _context.Users.FindAsync(userUpdateModel.Id);
            entity.Name = userUpdateModel.Name;
            _context.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<UserModel>> GetAll()
        {
            var entities = await _context.Users.ToListAsync();
            return entities;
        }

        public async Task<UserModel> GetByEmail(string email)
        {
            try
            {
                var entity = await _context.Users
                    .Where(x => x.Email.ToUpper() == email.ToUpper())
                    .FirstOrDefaultAsync();
                return entity;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<UserPasswordRequestModel> GetChangePasswordRequests(string email, string code)
        {
            var item = await _context.UserPasswordRequests
                .Include(x => x.User)
                .Where(x => x.Code.ToUpper() == code.ToUpper() &&
                            x.User.Email.ToUpper() == email.ToUpper())
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefaultAsync();
            return item;
        }

        public async Task SavePassword(Guid userId, string hash, string salt)
        {
            var item = await _context.Users.FindAsync(userId);
            item.Hash = hash;
            item.Salt = salt;
            _context.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task<UserPasswordRequestModel> AddPasswordRequest(Guid userId, string code)
        {
            var user = await GetById(userId);
            var newRequest = new UserPasswordRequestModel
            {
                UserId = user.Id,
                Code = code,
                CreatedDate = DateTime.UtcNow,
                IsActivated = false
            };
            await _context.UserPasswordRequests.AddAsync(newRequest);
            await _context.SaveChangesAsync();
            return newRequest;
        }

        public async Task ActivateChangePasswordRequestsCode(string code)
        {
            var item = await _context.UserPasswordRequests
                .Where(x => x.Code == code)
                .FirstOrDefaultAsync();
            item.ActivatedDate = DateTime.UtcNow;
            item.IsActivated = true;
            _context.UserPasswordRequests.Update(item);
            await _context.SaveChangesAsync();
        }
    }
}
