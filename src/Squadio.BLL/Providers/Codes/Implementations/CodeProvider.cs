using System;

namespace Squadio.BLL.Providers.Codes.Implementation
{
    public class CodeProvider : ICodeProvider
    {
        public string GenerateNumberCode(int length = 6)
        {
            var generator = new Random();
            
            var result = "";
            
            while (result.Length < length)
            {
                result += generator.Next(0, 9);
            }
            
            return result;
        }
        
        public string GenerateCodeAsGuid(int partsCount = 1)
        {
            var result = "";
            
            while (result.Length < partsCount)
            {
                var code = Guid.NewGuid();
                result += code.ToString("N");
            }
            
            return result;
        }
    }
}