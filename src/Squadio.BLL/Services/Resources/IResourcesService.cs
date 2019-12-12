using System;
using System.Threading.Tasks;
using Squadio.Common.Enums;
using Squadio.DTO.Resources;

namespace Squadio.BLL.Services.Resources
{
    public interface IResourcesService
    {
        Task CreateResource(Guid userId, FileGroup group, ResourceCreateDTO model);
        Task CreateImageResource(Guid userId, FileGroup group, ResourceImageCreateDTO model);
    }
}