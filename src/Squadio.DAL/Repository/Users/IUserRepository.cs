﻿using System;
using System.Threading.Tasks;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL.Repository.Users
{
    public interface IUserRepository : IBaseRepository<UserModel>
    {
        Task<UserModel> GetByEmail(string email);
        Task AddPasswordRequest(Guid userId, string code);
    }
}
