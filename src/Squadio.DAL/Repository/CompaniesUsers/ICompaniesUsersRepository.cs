﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Companies;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Pages;

namespace Squadio.DAL.Repository.CompaniesUsers
{
    public interface ICompaniesUsersRepository
    {
        Task<PageModel<UserModel>> GetCompanyUsers(Guid companyId, PageModel model);
        Task<CompanyUserModel> GetCompanyUser(Guid companyId, Guid userId);
        Task AddCompanyUser(Guid companyId, Guid userId, UserStatus userStatus);
        Task AddRangeCompanyUser(Guid companyId, IEnumerable<Guid> userIds, UserStatus userStatus);
        Task ChangeStatusCompanyUser(Guid companyId, Guid userId, UserStatus newUserStatus);
    }
}