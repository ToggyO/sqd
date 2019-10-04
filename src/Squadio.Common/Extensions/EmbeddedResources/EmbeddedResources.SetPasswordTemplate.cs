using System.Threading.Tasks;

namespace Squadio.Common.Extensions.EmbeddedResources
{
    public static partial class EmbeddedResources
    {
        public static string SetPasswordTemplate => "Api.BLL.Templates.SetPasswordTemplate.html";

        public static Task<string> GetSetPasswordTemplateAsync()
        {
            return GetResourceAsync(SetPasswordTemplate);
        }

        public static string GetSetPasswordTemplate()
        {
            return GetResource(SetPasswordTemplate);
        }
    }
}