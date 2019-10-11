using Squadio.DAL.Repository.Companies;

namespace Squadio.BLL.Providers.Companies.Implementation
{
    public class CompaniesProvider : ICompaniesProvider
    {
        private readonly ICompaniesRepository _repository;
        public CompaniesProvider(ICompaniesRepository repository)
        {
            _repository = repository;
        }
    }
}