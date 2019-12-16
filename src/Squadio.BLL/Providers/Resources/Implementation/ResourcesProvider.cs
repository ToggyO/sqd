﻿using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Resources;
using Squadio.Common.Models.Responses;
using Squadio.Common.Settings;
using Squadio.DAL.Repository.Resources;
using Squadio.Domain.Models.Resources;

namespace Squadio.BLL.Providers.Resources.Implementation
{
    public class ResourcesProvider : IResourcesProvider
    {
        private readonly IResourcesRepository _repository;
        private readonly IOptions<FileTemplateUrlModel> _options;

        public ResourcesProvider(IResourcesRepository repository
            , IOptions<FileTemplateUrlModel> options)
        {
            _repository = repository;
            _options = options;
        }

        public async Task<Response<ResourceViewModel>> GetViewModelByFileName(string filename)
        {
            var resourceResponse = await GetModelByFileName(filename);
            if (!resourceResponse.IsSuccess)
            {
                return ErrorResponse.MapResponse<ResourceViewModel, ResourceModel>(resourceResponse);
            }
            var viewModel = new ResourceViewModel(resourceResponse.Data, _options.Value.FileTemplate);
            return new Response<ResourceViewModel>
            {
                Data = viewModel
            };
        }

        public async Task<Response<ResourceImageViewModel>> GetImageViewModelByFileName(string filename)
        {
            var resourceResponse = await GetModelByFileName(filename);
            if (!resourceResponse.IsSuccess)
            {
                return ErrorResponse.MapResponse<ResourceImageViewModel, ResourceModel>(resourceResponse);
            }
            var viewModel = new ResourceImageViewModel(resourceResponse.Data, _options.Value.ImageTemplate);
            return new Response<ResourceImageViewModel>
            {
                Data = viewModel
            };
        }

        public async Task<Response<ResourceModel>> GetModelByFileName(string filename)
        {
            var resource = await _repository.GetByFilename(filename);
            if (resource == null)
            {
                return new BusinessConflictErrorResponse<ResourceModel>(new Error
                {
                    Code = ErrorCodes.Common.NotFound,
                    Message = ErrorMessages.Common.NotFound,
                    Field = ErrorFields.Resource.FileName
                });
            }
            return new Response<ResourceModel>
            {
                Data = resource
            };
        }
    }
}