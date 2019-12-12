using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Mapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Squadio.BLL.Providers.Resources;
using Squadio.BLL.Services.Resources;
using Squadio.Common.Enums;
using Squadio.Common.Models.Resources;
using Squadio.DTO.Resources;

namespace Squadio.API.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/versions")]
    public class VersionController : ControllerBase
    {
        private const string Version = "0.3.3 b";
        private readonly ILogger<VersionController> _logger;
        private readonly IResourcesService _resourcesService;
        private readonly IResourcesProvider _resourcesProvider;
        private readonly IMapper _mapper;

        public VersionController(ILogger<VersionController> logger
            , IResourcesService resourcesService
            , IResourcesProvider resourcesProvider
            , IMapper mapper)
        {
            _logger = logger;
            _resourcesService = resourcesService;
            _resourcesProvider = resourcesProvider;
            _mapper = mapper;
        }

        /// <summary>
        /// Get current version of API
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public string GetVersion()
        {
            return Version;
        }
        
        [HttpPost("image")]
        [AllowAnonymous]
        public async Task CreateResource([FromForm, Required] FileImageCreateDTO dto)
        {
            await _resourcesService.CreateImageResource(new Guid("c3b0eee9-988a-405d-9090-d715071040a6")
                , FileGroup.Avatar
                , _mapper.Map<FileImageCreateDTO, ResourceImageCreateDTO>(dto));
        }
        
        [HttpGet("image/{filename}")]
        [AllowAnonymous]
        public async Task<object> GetResource([FromRoute] string filename)
        {
            var viewModel = await _resourcesProvider.GetViewModelByFileName(filename);
            var dto = _mapper.Map<ResourceViewModel, ResourceDTO>(viewModel);
            return dto;
        }
    }
}