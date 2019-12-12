using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Resources;
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

        public async Task<ResourceViewModel> GetViewModelByFileName(string filename)
        {
            var resource = await GetModelByFileName(filename);
            var viewModel = new ResourceViewModel(resource);
            return viewModel;
        }

        public async Task<ResourceModel> GetModelByFileName(string filename)
        {
            var resource = await _repository.GetByFilename(filename);
            return resource;
        }
    }
}