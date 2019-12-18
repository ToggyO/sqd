using System;
using System.Threading.Tasks;
using Squadio.Common.Enums;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Resources;

namespace Squadio.BLL.Services.Resources
{
    public interface IResourcesService
    {
        Task<Response<ResourceDTO>> CreateResource(Guid userId, FileGroup group, FileCreateDTO dto);
        Task<Response<ResourceDTO>> CreateResource(Guid userId, FileGroup group, ResourceCreateDTO dto);
        Task<Response<ResourceImageDTO>> CreateResource(Guid userId, FileGroup group, FileImageCreateDTO dto);
        Task<Response<ResourceImageDTO>> CreateResource(Guid userId, FileGroup group, ResourceImageCreateDTO dto);
        Task<Response> DeleteResource(string filename);
    }
}