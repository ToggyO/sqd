using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Squadio.API.Handlers.Resources
{
    public interface IFilesHandler
    {
        Task<FileContentResult> GetFile(string group, string filename, string resolution = null);
    }
}