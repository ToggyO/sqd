using System;
using System.Linq;
using System.Threading.Tasks;
using Squadio.Domain.Models;

namespace Squadio.DAL.Repository
{
    public interface IBaseRepository<T> where T : BaseModel
    {
        Task<T> GetById(Guid id);
        Task<T> Create(T entity);
        Task<T> Delete(Guid id);
        Task<T> Update(T entity);
    }
}
