namespace Squadio.BLL.Providers.Codes
{
    public interface ICodeProvider
    {
        string GenerateNumberCode(int length = 6);
        string GenerateCodeAsGuid(int partsCount = 1);
    }
}