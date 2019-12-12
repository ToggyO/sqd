using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Squadio.Common.Settings;

namespace Squadio.BLL.Services.Files.Implementation
{
    public class FilesService : IFilesService
    {
        private readonly ILogger<FilesService> _logger;
        private readonly IOptions<FileRootDirectoryModel> _options;
        
        public FilesService(ILogger<FilesService> logger
            , IOptions<FileRootDirectoryModel> options)
        {
            _logger = logger;
            _options = options;
        }
        
        public async Task UploadImageFile(string group, string resolution, string filename, byte[] data)
        {
            var path = GenerateImagePath(group, resolution, filename);
            await UploadFile(path, data);
        }
        
        public async Task UploadFile(string group, string filename, byte[] data)
        {
            var path = GenerateFilePath(group, filename);
            await UploadFile(path, data);
        }
        
        private string GenerateFilePath(string group, string filename)
        {
            var path = _options.Value.Path;

            if (string.IsNullOrEmpty(path))
            {
                _logger.LogError("Root folder for files not specified");
                throw new Exception("Root folder for files not specified");
            }

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (path.LastOrDefault() != '/')
                path += "/";
            
            if(string.IsNullOrEmpty(group))
            {
                _logger.LogError("Group folder for files not specified");
                throw new Exception("Group folder for files not specified");
            }

            path += group;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            if(string.IsNullOrEmpty(filename))
            {
                _logger.LogError("Filename not specified");
                throw new Exception("Filename not specified");
            }

            path += "/" + filename;

            return path;
        }

        private string GenerateImagePath(string group, string resolution, string filename)
        {
            var path = _options.Value.Path;

            if (string.IsNullOrEmpty(path))
            {
                _logger.LogError("Root folder for files not specified");
                throw new Exception("Root folder for files not specified");
            }

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (path.LastOrDefault() != '/')
                path += "/";
            
            if(string.IsNullOrEmpty(group))
            {
                _logger.LogError("Group folder for files not specified");
                throw new Exception("Group folder for files not specified");
            }

            path += group;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            if(string.IsNullOrEmpty(resolution))
            {
                _logger.LogError("Resolution folder for files not specified");
                throw new Exception("Resolution folder for files not specified");
            }

            path += "/" + resolution;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            if(string.IsNullOrEmpty(filename))
            {
                _logger.LogError("Filename not specified");
                throw new Exception("Filename not specified");
            }

            path += "/" + filename;

            return path;
        }

        private async Task UploadFile(string path, byte[] data)
        {
            if (data?.Length > 0)
                await File.WriteAllBytesAsync(path, data);
            else
            {
                _logger.LogError("Input file is empty. Not saved: " + path);
                throw new Exception("Input file is empty. Not saved: " + path);
            }
        }
    }
}