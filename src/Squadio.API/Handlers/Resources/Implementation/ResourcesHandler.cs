using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Squadio.BLL.Providers.Resources;

namespace Squadio.API.Handlers.Resources.Implementation
{
    public class ResourcesHandler : IResourcesHandler
    {
        private readonly IResourcesProvider _resourceProvider;

        public ResourcesHandler(IResourcesProvider resourceProvider)
        {
            _resourceProvider = resourceProvider;
        }

        public async Task<FileContentResult> GetFile(string group, string resolution, string filename)
        {
            var resource = await _resourceProvider.GetModelByFilename(filename);

            if (resource == null)
                throw new Exception("File not exist in DB");
            
            try
            {
                // TODO: Add path across settings
                var folderPath = "path to root folder";
                var filePath = $"{group}/{resolution}/{filename}";
                var fileBytes = System.IO.File.ReadAllBytes($"{folderPath}/{filePath}");
                return new FileContentResult(fileBytes, resource.ContentType);
            }
            catch 
            {
                throw new Exception("File not exist in file system");
            }
        }

    }
}