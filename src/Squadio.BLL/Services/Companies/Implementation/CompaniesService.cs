using System;
using System.Threading.Tasks;
using Mapper;
using Squadio.BLL.Services.Teams;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Companies;
using Squadio.DAL.Repository.CompaniesUsers;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Companies;
using Squadio.DTO.Companies;

namespace Squadio.BLL.Services.Companies.Implementation
{
    public class CompaniesService : ICompaniesService
    {
        private readonly ICompaniesRepository _repository;
        private readonly ICompaniesUsersRepository _companiesUsersRepository;
        private readonly ITeamsService _teamsService;
        private readonly IMapper _mapper;
        public CompaniesService(ICompaniesRepository repository
            , ICompaniesUsersRepository companiesUsersRepository
            , ITeamsService teamsService
            , IMapper mapper)
        {
            _repository = repository;
            _companiesUsersRepository = companiesUsersRepository;
            _teamsService = teamsService;
            _mapper = mapper;
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

            if (companyUser.Status != UserStatus.SuperAdmin)
            {
                return new PermissionDeniedErrorResponse<CompanyDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.PermissionDenied,
                        Message = ErrorMessages.Security.PermissionDenied
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

        public async Task<Response> DeleteUserFromCompany(Guid companyId, Guid removeUserId, Guid currentUserId)
        {
            var currentCompanyUser = await _companiesUsersRepository.GetCompanyUser(companyId, currentUserId);

            if (currentCompanyUser == null || currentCompanyUser?.Status != UserStatus.SuperAdmin)
            {
                return new PermissionDeniedErrorResponse(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.PermissionDenied,
                        Message = ErrorMessages.Security.PermissionDenied
                    }
                }); 
            }

            await _companiesUsersRepository.DeleteCompanyUser(companyId, removeUserId);
            await _teamsService.DeleteUserFromTeamsByCompanyId(companyId, removeUserId);
            
            return new Response();
        }

        public async Task<Response<CompanyDTO>> Create(Guid userId, CreateCompanyDTO dto)
        {
            var entityCompany = new CompanyModel
            {
                Name = dto.Name,
                Address = dto.Address,
                CreatedDate = DateTime.UtcNow,
                CreatorId = userId
            };
            
            entityCompany = await _repository.Create(entityCompany);

            await _companiesUsersRepository.AddCompanyUser(entityCompany.Id, userId, UserStatus.SuperAdmin);
            
            var result = _mapper.Map<CompanyModel, CompanyDTO>(entityCompany);
            
            return new Response<CompanyDTO>
            {
                Data = result
            };
        }
    }
}