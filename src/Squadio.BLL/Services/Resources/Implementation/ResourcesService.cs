using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Squadio.BLL.Services.Files;
using Squadio.BLL.Services.ImageResizeTools;
using Squadio.Common.Enums;
using Squadio.DAL.Repository.Resources;
using Squadio.Domain.Models.Resources;
using Squadio.DTO.Resources;

namespace Squadio.BLL.Services.Resources.Implementation
{
    public class ResourcesService : IResourcesService
    {
        private readonly IResourcesRepository _repository;
        private readonly IFilesService _filesService;

        public ResourcesService(IResourcesRepository repository
            , IFilesService filesService)
        {
            _repository = repository;
            _filesService = filesService;
        }

        public async Task CreateResource(Guid userId, FileGroup group, ResourceCreateDTO model)
        {
            await CreateResource(userId, group.ToString().ToLower(), model);
        }

        public async Task CreateImageResource(Guid userId, FileGroup group, ResourceImageCreateDTO model)
        {
            await CreateImageResource(userId, group.ToString().ToLower(), model);
        }
        
        private async Task CreateResource(Guid userId, string group, ResourceCreateDTO model)
        {
            var fileName = Guid.NewGuid().ToString("N");

            var resourceEntity = new ResourceModel
            {
                UserId = userId,
                Group = group,
                FileName = fileName,
                ContentType = model.ContentType,
                CreateDate = DateTime.UtcNow
            };

            var resource = await _repository.Create(resourceEntity);

            await _filesService.UploadFile(group, fileName, model.Bytes);
        }
        
        private async Task CreateImageResource(Guid userId, string group, ResourceImageCreateDTO model)
        {
            var fileName = Guid.NewGuid().ToString("N");

            var resourceEntity = new ResourceModel
            {
                UserId = userId,
                Group = group,
                FileName = fileName,
                ContentType = model.ContentType,
                CreateDate = DateTime.UtcNow
            };

            var resource = await _repository.Create(resourceEntity);

            await _filesService.UploadImageFile(group, "original", fileName, model.Bytes);

            //TODO: make across appsettings
            //var sizes = Options.Value.Sizes.Split(',');
            var sizes = new[] {"140", "360", "480", "720", "1080"};
            foreach (var size in sizes)
            {
                var image = ImageResizer.Resize(model.Bytes, int.Parse(size), model.ContentType);
                await _filesService.UploadImageFile(group, size, fileName, image);
            }
        }
    }
}