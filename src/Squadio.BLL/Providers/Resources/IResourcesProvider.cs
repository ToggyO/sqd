using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Resources;
using Squadio.Domain.Models.Resources;

namespace Squadio.BLL.Providers.Resources
{
    public interface IResourcesProvider
    {
        Task<ResourceViewModel> GetViewModelByFileName(string filename);
        Task<ResourceModel> GetModelByFileName(string filename);
    }
}