using System.Threading.Tasks;
using Squadio.Domain.Models.Resources;

namespace Squadio.BLL.Providers.Resources
{
    public interface IResourcesProvider
    {
        Task<ResourceModel> GetModelByFilename(string filename);
    }
}