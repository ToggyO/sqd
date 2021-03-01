using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Squadio.BLL.Services.Files;
using Squadio.BLL.Services.ImageResizeTools;
using Squadio.Common.Enums;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Resources;
using Squadio.Common.Models.Responses;
using Squadio.Common.Settings.Static;
using Squadio.DAL.Repository.Resources;
using Squadio.Domain.Models.Resources;
using Squadio.DTO.Models.Resources;

namespace Squadio.BLL.Services.Resources.Implementations
{
    public class ResourcesService : IResourcesService
    {
        private readonly IResourcesRepository _repository;
        private readonly IFilesService _filesService;
        private readonly IMapper _mapper;
        private readonly ILogger<ResourcesService> _logger;

        public ResourcesService(IResourcesRepository repository
            , IFilesService filesService
            , IMapper mapper
            , ILogger<ResourcesService> logger)
        {
            _repository = repository;
            _filesService = filesService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Response<ResourceDTO>> CreateFileResource(Guid userId, FileGroup @group, FileCreateDTO dto)
        {
            var resource = await CreateFileResource(userId, group.ToString().ToLower(), dto);
            var viewModel = new ResourceViewModel(resource);
            var result = _mapper.Map<ResourceViewModel, ResourceDTO>(viewModel);
            return new Response<ResourceDTO>
            {
                Data = result
            };
        }

        public async Task<Response<ResourceImageDTO>> CreateImageResource(Guid userId, FileGroup @group, ImageCreateDTO dto)
        {
            var resource = await CreateImageResource(userId, group.ToString().ToLower(), dto);
            var viewModel = new ResourceImageViewModel(resource);
            var result = _mapper.Map<ResourceImageViewModel, ResourceImageDTO>(viewModel);
            return new Response<ResourceImageDTO>
            {
                Data = result
            };
        }

        public async Task<Response> DeleteResource(string filename)
        {
            var entity = await _repository.GetByFilename(filename);
            if(entity == null)
                return new BusinessConflictErrorResponse(new Error
                {
                    Code = ErrorCodes.Common.NotFound,
                    Field = ErrorFields.Resource.FileName,
                    Message = ErrorMessages.Common.NotFound
                });

            if (entity.IsWithResolution)
                await DeleteImageResource(entity);
            else
                await DeleteResource(entity);
            
            return new Response();
        }

        public async Task<Response> DeleteResource(Guid resourceId)
        {
            var entity = await _repository.GetById(resourceId);
            if(entity == null)
                return new BusinessConflictErrorResponse(new Error
                {
                    Code = ErrorCodes.Common.NotFound,
                    Field = ErrorFields.Resource.FileName,
                    Message = ErrorMessages.Common.NotFound
                });

            if (entity.IsWithResolution)
                await DeleteImageResource(entity);
            else
                await DeleteResource(entity);

            await _repository.Delete(entity.Id);
            
            return new Response();
        }

        private async Task<ResourceModel> CreateFileResource(Guid userId, string group, FileCreateDTO dto)
        {
            var fileName = Guid.NewGuid().ToString("N");

            var resourceEntity = new ResourceModel
            {
                UserId = userId,
                Group = group,
                FileName = fileName,
                ContentType = dto.ContentType,
                CreatedDate = DateTime.UtcNow,
                IsWithResolution = false
            };

            var resource = await _repository.Create(resourceEntity);

            await _filesService.UploadFile(group, fileName, dto.Stream);
            
            await dto.Stream.DisposeAsync();

            return resource;
        }
        
        private async Task<ResourceModel> CreateImageResource(Guid userId, string group, ImageCreateDTO dto)
        {
            var fileName = Guid.NewGuid().ToString("N");

            var resourceEntity = new ResourceModel
            {
                UserId = userId,
                Group = group,
                FileName = fileName,
                ContentType = dto.ContentType,
                CreatedDate = DateTime.UtcNow,
                IsWithResolution = true
            };

            var resource = await _repository.Create(resourceEntity);
            
            await _filesService.UploadImageFile(group, "original", fileName, dto.Stream);

            var baseImage = ImageResizer.GetBaseImage(dto.Stream);

            foreach (var size in CropSizesSettings.Sizes)
            {
                var image = ImageResizer.GetResizedImage(baseImage, size, dto.ContentType);
                await _filesService.UploadImageFile(group, size.ToString(), fileName, image);
                image.Dispose();
            }
            
            dto.Stream.Dispose();

            return resource;
        }
        
        private async Task DeleteResource(ResourceModel model)
        {
            await _filesService.DeleteFile(model.Group, model.FileName);
        }
        
        private async Task DeleteImageResource(ResourceModel model)
        {
            await _filesService.DeleteImageFile(model.Group, model.FileName);
        }
    }
}