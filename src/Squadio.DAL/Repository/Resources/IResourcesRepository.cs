using System;
using System.Threading.Tasks;
using Squadio.Domain.Models.Resources;

namespace Squadio.DAL.Repository.Resources
{
    public interface IResourcesRepository
    {
        Task<ResourceModel> Create(ResourceModel model);
        Task<ResourceModel> GetById(Guid id);
        Task<ResourceModel> GetByFilename(string filename);
    }
}