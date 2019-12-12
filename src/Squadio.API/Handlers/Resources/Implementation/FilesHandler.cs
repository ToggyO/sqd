﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Squadio.BLL.Providers.Resources;
using Squadio.BLL.Services.Files.Implementation;
using Squadio.Common.Settings;

namespace Squadio.API.Handlers.Resources.Implementation
{
    public class FilesHandler : IFilesHandler
    {
        private readonly IResourcesProvider _resourceProvider;
        private readonly IOptions<FileRootDirectoryModel> _options;
        private readonly ILogger<FilesService> _logger;
        

        public FilesHandler(IResourcesProvider resourceProvider
            , IOptions<FileRootDirectoryModel> options
            , ILogger<FilesService> logger)
        {
            _resourceProvider = resourceProvider;
            _options = options;
            _logger = logger;
        }

        public async Task<FileContentResult> GetFile(string group, string filename, string resolution = null)
        {
            var resource = await _resourceProvider.GetModelByFileName(filename);

            if (resource == null)
                throw new Exception("File not exist in DB");
            
            try
            {
                var pathToFile = GetFilePath(group, filename, resolution);
                var fileBytes = System.IO.File.ReadAllBytes(pathToFile);
                return new FileContentResult(fileBytes, resource.ContentType);
            }
            catch 
            {
                throw new Exception("File not exist in file system");
            }
        }

        private string GetFilePath(string group, string filename, string resolution = null)
        {
            var path = _options.Value.Path;
            
            if(!Directory.Exists(path))
            {
                _logger.LogError("Root folder with files not exist");
                throw new Exception("Root folder with files not exist");
            }

            if (path.LastOrDefault() != '/')
                path += "/";

            path += group;
            
            if(!Directory.Exists(path))
            {
                _logger.LogError("Group folder with files not exist");
                throw new Exception("Group folder with files not exist");
            }


            if (!string.IsNullOrEmpty(resolution))
            {
                path += "/" + resolution;
                
                if(!Directory.Exists(path))
                {
                    _logger.LogError("Resolution folder with files not exist");
                    throw new Exception("Resolution folder with files not exist");
                }
            }
            
            if(string.IsNullOrEmpty(filename))
            {
                _logger.LogError("FileName not specified");
                throw new Exception("FileName not specified");
            }

            path += "/" + filename;

            return path;
        }

    }
}