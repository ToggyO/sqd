using System;
using System.Threading.Tasks;
using Squadio.Domain.Enums;

namespace Squadio.DAL.Repository.CompaniesUsers
{
    public interface ICompaniesUsersRepository
    {
        Task AddCompanyUser(Guid companyId, Guid userId, UserStatus userStatus);
        Task ChangeStatusCompanyUser(Guid companyId, Guid userId, UserStatus newUserStatus);
    }
}