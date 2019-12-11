using System.Threading.Tasks;
using Squadio.DAL.Repository.Resources;
using Squadio.Domain.Models.Resources;

namespace Squadio.BLL.Providers.Resources.Implementation
{
    public class ResourcesProvider : IResourcesProvider
    {
        private readonly IResourcesRepository _repository;

        public ResourcesProvider(IResourcesRepository repository)
        {
            _repository = repository;
        }
        
        public Task<ResourceModel> GetModelByFilename(string filename)
        {
            throw new System.NotImplementedException();
        }
    }
}