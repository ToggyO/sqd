using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Squadio.Common.Settings;

namespace Squadio.BLL.Services.Files.Implementations
{
    public class FilesService : IFilesService
    {
        private readonly ILogger<FilesService> _logger;
        private readonly IOptions<FileRootDirectorySettings> _options;
        
        public FilesService(ILogger<FilesService> logger
            , IOptions<FileRootDirectorySettings> options)
        {
            _logger = logger;
            _options = options;
        }

        public async Task UploadImageFile(string group, string resolution, string filename, Stream stream)
        {
            var path = GenerateImagePath(group, resolution, filename);
            await UploadFile(path, stream);
        }

        public async Task DeleteImageFile(string group, string filename)
        {
            foreach (var size in CropSizesSettings.Sizes)
            {
                var path = GenerateImagePath(group, size.ToString(), filename);
                await DeleteFileSystem(path);
            }
            var originalPath = GenerateImagePath(group, "original", filename);
            await DeleteFileSystem(originalPath);
        }

        public async Task UploadFile(string group, string filename, Stream stream)
        {
            var path = GenerateFilePath(group, filename);
            await UploadFile(path, stream);
        }

        public async Task DeleteFile(string group, string filename)
        {
            var path = GenerateFilePath(group, filename);
            await DeleteFileSystem(path);
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
                throw new Exception("Root folder for files not specified");
            }

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (path.LastOrDefault() != '/')
                path += "/";
            
            if(string.IsNullOrEmpty(group))
            {
                throw new Exception("Group folder for files not specified");
            }

            path += group;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            if(string.IsNullOrEmpty(resolution))
            {
                throw new Exception("Resolution folder for files not specified");
            }

            path += "/" + resolution;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            if(string.IsNullOrEmpty(filename))
            {
                throw new Exception("Filename not specified");
            }

            path += "/" + filename;

            return path;
        }

        private async Task UploadFile(string path, Stream stream)
        {
            using (var fileStream = File.Create(path))
            {
                stream.Seek(0, SeekOrigin.Begin);
                await stream.CopyToAsync(fileStream);
            }
        }

        private async Task DeleteFileSystem(string path)
        {
            
            await Task.Run(() =>
            {
                try
                {
                    var fileInf = new FileInfo(path);
                    if (fileInf.Exists)
                    {
                        fileInf.Delete();
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"File {path} not removed cause: {e.Message}");
                }
            });
        }
    }
}