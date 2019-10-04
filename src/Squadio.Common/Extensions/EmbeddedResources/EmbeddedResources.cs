using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Squadio.Common.Extensions.EmbeddedResources
{
    public static partial class EmbeddedResources
    {
        public static async Task<string> GetResourceAsync(string key)
        {
            try
            {
                using (var stream = typeof(EmbeddedResources).GetTypeInfo().Assembly.GetManifestResourceStream(key) ?? throw new Exception("Manifest resource stream is null"))
                using (var reader = new StreamReader(stream, Encoding.UTF8)) return await reader.ReadToEndAsync();
            }

            catch (Exception exception)
            {
                throw new Exception($"Failed to read Embedded Resource {key}", innerException: exception);
            }
        }

        public static string GetResource(string key)
        {
            try
            {
                using (var stream = typeof(EmbeddedResources).GetTypeInfo().Assembly.GetManifestResourceStream(key) ?? throw new Exception("Manifest resource stream is null"))
                using (var reader = new StreamReader(stream, Encoding.UTF8)) return reader.ReadToEnd();
            }

            catch (Exception exception)
            {
                throw new Exception($"Failed to read Embedded Resource {key}", innerException: exception);
            }
        }
    }
}