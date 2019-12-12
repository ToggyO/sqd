using System;
using System.Threading.Tasks;
using Mapper;
using Microsoft.Extensions.Options;
using Squadio.BLL.Services.Files;
using Squadio.BLL.Services.ImageResizeTools;
using Squadio.Common.Enums;
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

        public ResourcesService(IResourcesRepository repository
            , IFilesService filesService
            , IMapper mapper
            , IOptions<FileTemplateUrlModel> options)
        {
            _repository = repository;
            _filesService = filesService;
            _mapper = mapper;
            _options = options;
        }

        public async Task<Response<ResourceDTO>> CreateResource(Guid userId, FileGroup group, FileCreateDTO dto)
        {
            var resourceCreateDTO = _mapper.Map<FileCreateDTO, ResourceCreateDTO>(dto);
            var resource = await CreateResource(userId, group.ToString().ToLower(), resourceCreateDTO);
            var viewModel = new ResourceViewModel(resource, _options.Value.Template);
            var result = _mapper.Map<ResourceViewModel, ResourceDTO>(viewModel);
            return new Response<ResourceDTO>
            {
                Data = result
            };
        }

        public async Task<Response<ResourceDTO>> CreateResource(Guid userId, FileGroup group, ResourceCreateDTO dto)
        {
            var resource = await CreateResource(userId, group.ToString().ToLower(), dto);
            var viewModel = new ResourceViewModel(resource, _options.Value.Template);
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
            var viewModel = new ResourceViewModel(resource, _options.Value.Template);
            var result = _mapper.Map<ResourceViewModel, ResourceImageDTO>(viewModel);
            return new Response<ResourceImageDTO>
            {
                Data = result
            };
        }

        public async Task<Response<ResourceImageDTO>> CreateResource(Guid userId, FileGroup group, ResourceImageCreateDTO dto)
        {
            var resource = await CreateResource(userId, group.ToString().ToLower(), dto);
            var viewModel = new ResourceViewModel(resource, _options.Value.Template);
            var result = _mapper.Map<ResourceViewModel, ResourceImageDTO>(viewModel);
            return new Response<ResourceImageDTO>
            {
                Data = result
            };
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
                CreateDate = DateTime.UtcNow
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
                CreateDate = DateTime.UtcNow
            };

            var resource = await _repository.Create(resourceEntity);

            await _filesService.UploadImageFile(group, "original", fileName, dto.Bytes);

            var sizes = new[] {"140", "360", "480", "720", "1080"};
            foreach (var size in sizes)
            {
                var image = ImageResizer.Resize(dto.Bytes, int.Parse(size), dto.ContentType);
                await _filesService.UploadImageFile(group, size, fileName, image);
            }

            return resource;
        }
    }
}