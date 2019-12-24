﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Logging;
using Mapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Squadio.BLL.Services.Files;
using Squadio.BLL.Services.ImageResizeTools;
using Squadio.Common.Enums;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Resources;
using Squadio.Common.Models.Responses;
using Squadio.Common.Settings;
using Squadio.DAL.Repository.Resources;
using Squadio.Domain.Models.Resources;
using Squadio.DTO.Resources;

namespace Squadio.BLL.Services.Resources.Implementation
{
    public class ResourcesService : IResourcesService
    {
        private readonly IResourcesRepository _repository;
        private readonly IFilesService _filesService;
        private readonly IMapper _mapper;
        private readonly IOptions<FileTemplateUrlModel> _options;
        private readonly IOptions<CropSizesModel> _sizeOptions;
        private readonly ILogger<ResourcesService> _logger;

        public ResourcesService(IResourcesRepository repository
            , IFilesService filesService
            , IMapper mapper
            , IOptions<FileTemplateUrlModel> options
            , IOptions<CropSizesModel> sizeOptions
            , ILogger<ResourcesService> logger)
        {
            _repository = repository;
            _filesService = filesService;
            _mapper = mapper;
            _options = options;
            _sizeOptions = sizeOptions;
            _logger = logger;
        }

        public async Task<Response<ResourceDTO>> CreateResource(Guid userId, FileGroup group, FileCreateDTO dto)
        {
            var resourceCreateDTO = _mapper.Map<FileCreateDTO, ResourceCreateDTO>(dto);
            var resource = await CreateResource(userId, group.ToString().ToLower(), resourceCreateDTO);
            var viewModel = new ResourceViewModel(resource, _options.Value.FileTemplate);
            var result = _mapper.Map<ResourceViewModel, ResourceDTO>(viewModel);
            return new Response<ResourceDTO>
            {
                Data = result
            };
        }

        public async Task<Response<ResourceDTO>> CreateResource(Guid userId, FileGroup group, ResourceCreateDTO dto)
        {
            var resource = await CreateResource(userId, group.ToString().ToLower(), dto);
            var viewModel = new ResourceViewModel(resource, _options.Value.FileTemplate);
            var result = _mapper.Map<ResourceViewModel, ResourceDTO>(viewModel);
            return new Response<ResourceDTO>
            {
                Data = result
            };
        }

        public async Task<Response<ResourceImageDTO>> CreateResource(Guid userId, FileGroup group, FileImageCreateDTO dto)
        {
            var resourceImageCreateDTO = _mapper.Map<FileImageCreateDTO, ResourceImageCreateDTO>(dto);
            var resource = await CreateResource(userId, group.ToString().ToLower(), resourceImageCreateDTO);
            var viewModel = new ResourceImageViewModel(resource, _options.Value.ImageTemplate);
            var result = _mapper.Map<ResourceImageViewModel, ResourceImageDTO>(viewModel);
            return new Response<ResourceImageDTO>
            {
                Data = result
            };
        }

        public async Task<Response<ResourceImageDTO>> CreateResource(Guid userId, FileGroup group, ResourceImageCreateDTO dto)
        {
            var resource = await CreateResource(userId, group.ToString().ToLower(), dto);
            var viewModel = new ResourceImageViewModel(resource, _options.Value.ImageTemplate);
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

        private async Task<ResourceModel> CreateResource(Guid userId, string group, ResourceCreateDTO dto)
        {
            var fileName = Guid.NewGuid().ToString("N");

            var resourceEntity = new ResourceModel
            {
                UserId = userId,
                Group = group,
                FileName = fileName,
                ContentType = dto.ContentType,
                CreateDate = DateTime.UtcNow,
                IsWithResolution = false
            };

            var resource = await _repository.Create(resourceEntity);

            await _filesService.UploadFile(group, fileName, dto.Bytes);

            return resource;
        }
        
        private async Task<ResourceModel> CreateResource(Guid userId, string group, ResourceImageCreateDTO dto)
        {
            var fileName = Guid.NewGuid().ToString("N");

            var resourceEntity = new ResourceModel
            {
                UserId = userId,
                Group = group,
                FileName = fileName,
                ContentType = dto.ContentType,
                CreateDate = DateTime.UtcNow,
                IsWithResolution = true
            };

            var resource = await _repository.Create(resourceEntity);

            await _filesService.UploadImageFile(group, "original", fileName, dto.Bytes);
            
            var sizes = _sizeOptions.Value.Sizes;
            
            if(sizes == null)
                _logger.LogError("sizes is null");
            else
            {
                _logger.LogError($"_sizeOptions.Value.CropSizes str = {_sizeOptions.Value.CropSizes}");
                if (sizes.Length == 0)
                    _logger.LogError($"sizes not null but empty!");
                if (sizes.Length > 0)
                    _logger.LogError($"sizes[0] = {sizes[0]}");
            }


            foreach (var size in sizes)
            {
                var image = ImageResizer.Resize(dto.Bytes, size, dto.ContentType);
                await _filesService.UploadImageFile(group, size.ToString(), fileName, image);
            }

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