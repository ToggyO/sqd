using System;
using System.Threading.Tasks;
using Squadio.Common.Enums;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Models.Resources;

namespace Squadio.BLL.Services.Resources
{
    public interface IResourcesService
    {
        Task<Response<ResourceDTO>> CreateFileResource(Guid userId, FileGroup group, FileCreateDTO dto);
        Task<Response<ResourceImageDTO>> CreateImageResource(Guid userId, FileGroup group, ImageCreateDTO dto);
        Task<Response> DeleteResource(string filename);
        Task<Response> DeleteResource(Guid resourceId);
    }
}