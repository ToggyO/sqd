﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Squadio.Common.Models.Pages;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL.Repository.Users.Implementations
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

        public async Task<UserModel> Delete(Guid id)
        {
            var entity = await _context.Users.FindAsync(id);
            entity.IsDeleted = true;
            return await Update(entity);
        }

        public async Task<UserModel> GetById(Guid id)
        {
            var entity = await _context.Users
                .Where(x => !x.IsDeleted)
                .Include(x => x.Role)
                .Include(x => x.Avatar)
                .FirstOrDefaultAsync(x => x.Id == id);
            return entity;
        }

        public async Task<UserModel> Update(UserModel entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<PageModel<UserModel>> GetPage(PageModel model)
        {
            var total = await _context.Users.CountAsync();
            var items = await _context.Users
                .Where(x => !x.IsDeleted)
                .Include(x => x.Role)
                .Include(x => x.Avatar)
                .Skip((model.Page - 1) * model.PageSize)
                .Take(model.PageSize)
                .ToListAsync();
            var result = new PageModel<UserModel>
            {
                Page = model.Page,
                PageSize = model.PageSize,
                Total = total,
                Items = items
            };
            return result;
        }

        public async Task<UserModel> GetByEmail(string email)
        {
            if (email == null) return null;
            
            var entity = await _context.Users
                .Where(x => !x.IsDeleted)
                .Include(x => x.Role)
                .Include(x => x.Avatar)
                .FirstOrDefaultAsync(x => x.Email.ToUpper() == email.ToUpper());
            return entity;
        }

        public async Task SavePassword(Guid userId, string hash, string salt)
        {
            var item = await _context.Users.FindAsync(userId);
            item.Hash = hash;
            item.Salt = salt;
            _context.Update(item);
            await _context.SaveChangesAsync();
        }
    }
}