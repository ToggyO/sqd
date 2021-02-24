using System.IO;
using System.Threading.Tasks;

namespace Squadio.BLL.Services.Files
{
    public interface IFilesService
    {
        Task UploadImageFile(string group, string resolution, string filename, Stream stream);
        Task DeleteImageFile(string group, string filename);
        Task UploadFile(string group, string filename, Stream stream);
        Task DeleteFile(string group, string filename);
    }
}