using Squadio.DAL.Repository.Companies;

namespace Squadio.BLL.Services.Companies.Implementation
{
    public class CompaniesService : ICompaniesService
    {
        private readonly ICompaniesRepository _repository;
        public CompaniesService(ICompaniesRepository repository)
        {
            _repository = repository;
        }
    }
}