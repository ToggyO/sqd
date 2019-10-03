﻿using System;
using System.Linq;
using System.Threading.Tasks;

namespace Squadio.DAL.Repository
{
    public interface IBaseRepository<T>
    {
        Task<T> Get(Guid id);
        Task<T> Create(T entity);
        Task<T> Delete(Guid id);
        Task<T> Update(T entity);
    }
}
