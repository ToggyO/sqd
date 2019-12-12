using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Resources;
using Squadio.Common.Models.Responses;
using Squadio.Domain.Models.Resources;

namespace Squadio.BLL.Providers.Resources
{
    public interface IResourcesProvider
    {
        Task<Response<ResourceViewModel>> GetViewModelByFileName(string filename);
        Task<Response<ResourceImageViewModel>> GetImageViewModelByFileName(string filename);
        Task<Response<ResourceModel>> GetModelByFileName(string filename);
    }
}