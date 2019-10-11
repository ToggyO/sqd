using Squadio.BLL.Providers.Companies;
using Squadio.BLL.Services.Companies;

namespace Squadio.API.Handlers.Companies.Implementation
{
    public class CompaniesHandler : ICompaniesHandler
    {
        private readonly ICompaniesProvider _provider;
        private readonly ICompaniesService _service;
        
        public CompaniesHandler(ICompaniesProvider provider
            , ICompaniesService service)
        {
            _provider = provider;
            _service = service;
        }
    }
}