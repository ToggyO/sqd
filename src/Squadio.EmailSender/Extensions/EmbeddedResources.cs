using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Squadio.EmailSender.Extensions
{
    public class EmbeddedResources
    {
        public static string GetResource(string key)
        {
            return GetResource(typeof(EmbeddedResources).Assembly, key);
        }
        
        public static string GetResource(Assembly assembly, string key)
        {
            try
            {
                using (var stream = assembly.GetManifestResourceStream(key) ?? throw new Exception("Manifest resource stream is null"))
                {
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to read Embedded Resource {key}", innerException: exception);
            }
        }
        
        public static async Task<string> GetResourceAsync(string key)
        {
            return await GetResourceAsync(typeof(EmbeddedResources).GetTypeInfo().Assembly, key);
        }
        
        public static async Task<string> GetResourceAsync(Assembly assembly, string key)
        {
            try
            {
                using (var stream = assembly.GetManifestResourceStream(key) ?? throw new Exception("Manifest resource stream is null"))
                {
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        return await reader.ReadToEndAsync();
                    }
                }
            }

            catch (Exception exception)
            {
                throw new Exception($"Failed to read Embedded Resource {key}", innerException: exception);
            }
        }
    }
}