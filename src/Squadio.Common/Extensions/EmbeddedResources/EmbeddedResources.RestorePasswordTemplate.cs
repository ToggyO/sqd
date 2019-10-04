using System.Threading.Tasks;

namespace Squadio.Common.Extensions.EmbeddedResources
{
    public static partial class EmbeddedResources
    {
        public static string RestorePasswordTemplate => "Api.BLL.Templates.RestorePasswordTemplate.html";

        public static Task<string> GetRestorePasswordTemplateAsync()
        {
            return GetResourceAsync(RestorePasswordTemplate);
        }

        public static string GetRestorePasswordTemplate()
        {
            return GetResource(RestorePasswordTemplate);
        }
    }
}