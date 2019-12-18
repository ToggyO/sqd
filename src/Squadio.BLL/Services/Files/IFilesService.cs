using System.Threading.Tasks;
using Squadio.DTO.Resources;

namespace Squadio.BLL.Services.Files
{
    public interface IFilesService
    {
        Task UploadImageFile(string group, string resolution, string filename, byte[] data);
        Task DeleteImageFile(string group, string filename);
        Task UploadFile(string group, string filename, byte[] data);
        Task DeleteFile(string group, string filename);
    }
}