using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.DAL.Extensions;
using Squadio.Domain.Enums;
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

        public async Task<PageModel<UserModel>> GetPage(PageModel model, UserFilterModel filter = null)
        {
            var query = _context.Users
                .Include(x => x.Role)
                .Include(x => x.Avatar)
                .AsQueryable();

            if (filter != null)
            {
                //TODO: Filter

                if (!string.IsNullOrEmpty(filter.Search))
                {
                    query = query.Where(x => 
                        x.Name.ToUpper().Contains(filter.Search.ToUpper())
                        || x.Email.ToUpper().Contains(filter.Search.ToUpper()));
                }

                if (filter.UserStatus != null)
                {
                    query = query.Where(x => x.Status == filter.UserStatus.Value);
                }

                if (filter.IncludeDeleted != true)
                {
                    query = query.Where(x => !x.IsDeleted);
                }

                if (filter.IncludeAdmin != true)
                {
                    query = query.Where(x => x.RoleId != RoleGuid.Admin);
                }
            }
            
            var total = await query.CountAsync();
            
            var items = await query
                .GetPage(model)
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
