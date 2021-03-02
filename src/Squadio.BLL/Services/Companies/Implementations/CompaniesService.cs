using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Squadio.BLL.Services.Membership;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Companies;
using Squadio.DAL.Repository.CompaniesUsers;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Companies;
using Squadio.DTO.Models.Companies;

namespace Squadio.BLL.Services.Companies.Implementations
{
    public class CompaniesService : ICompaniesService
    {
        private readonly ICompaniesRepository _repository;
        private readonly ICompaniesUsersRepository _companiesUsersRepository;
        private readonly IMembershipService _membershipService;
        private readonly IMapper _mapper;
        private readonly ILogger<CompaniesService> _logger;
        public CompaniesService(ICompaniesRepository repository
            , ICompaniesUsersRepository companiesUsersRepository
            , IMembershipService membershipService
            , IMapper mapper
            , ILogger<CompaniesService> logger)
        {
            _repository = repository;
            _companiesUsersRepository = companiesUsersRepository;
            _membershipService = membershipService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Response<CompanyDTO>> Update(Guid companyId, Guid userId, CompanyUpdateDTO dto)
        {
            var companyUser = await _companiesUsersRepository.GetCompanyUser(companyId, userId);
            
            if (companyUser == null)
            {
                return new BusinessConflictErrorResponse<CompanyDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Common.NotFound,
                        Message = ErrorMessages.Common.NotFound,
                        Field = "userId"
                    }
                });
            }

            if (companyUser.Status != MembershipStatus.SuperAdmin)
            {
                return new ForbiddenErrorResponse<CompanyDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.Forbidden,
                        Message = ErrorMessages.Security.Forbidden
                    }
                }); 
            }
            
            var companyEntity = await _repository.GetById(companyId);
            
            if (companyEntity == null)
            {
                return new BusinessConflictErrorResponse<CompanyDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Common.NotFound,
                        Message = ErrorMessages.Common.NotFound,
                        Field = "companyId"
                    }
                });
            }
            
            companyEntity.Name = dto.Name;
            companyEntity.Address = dto.Address;
            
            companyEntity = await _repository.Update(companyEntity);
            
            var result = _mapper.Map<CompanyModel, CompanyDTO>(companyEntity);
            
            return new Response<CompanyDTO>
            {
                Data = result
            };
        }

        public async Task<Response<CompanyDTO>> Create(Guid userId, CompanyCreateDTO dto)
        {
            var entityCompany = new CompanyModel
            {
                Name = dto.Name,
                Address = dto.Address,
                CreatedDate = DateTime.UtcNow,
                CreatorId = userId
            };
            
            entityCompany = await _repository.Create(entityCompany);

            await _companiesUsersRepository.AddCompanyUser(entityCompany.Id, userId, MembershipStatus.SuperAdmin);
            
            var result = _mapper.Map<CompanyModel, CompanyDTO>(entityCompany);
            
            return new Response<CompanyDTO>
            {
                Data = result
            };
        }
    }
}