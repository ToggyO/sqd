using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.CompaniesUsers;
using Squadio.DAL.Repository.Users;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Models.Companies;
using Squadio.DTO.Models.Users;

namespace Squadio.BLL.Providers.Admins.Implementations
{
    public class AdminsProvider : IAdminsProvider
    {
        private readonly IUsersRepository _repository;
        private readonly ICompaniesUsersRepository _companiesUsersRepository;
        private readonly IMapper _mapper;

        public AdminsProvider(IUsersRepository repository
            , ICompaniesUsersRepository companiesUsersRepository
            , IMapper mapper)
        {
            _repository = repository;
            _companiesUsersRepository = companiesUsersRepository;
            _mapper = mapper;
        }
        
        public async Task<Response<PageModel<UserWithCompaniesDTO>>> GetUsersPage(PageModel model, UserFilterAdminDTO filter)
        {
            var mappedFilter = _mapper.Map<UserFilterModel>(filter);
            var usersPage = await _repository.GetPage(model, mappedFilter);
            
            var mapped = _mapper.Map<PageModel<UserWithCompaniesDTO>>(usersPage);

            var usersCompaniesList = (await _companiesUsersRepository.GetCompaniesByUsers(
                mapped.Items.Select(x => x.User.Id))).ToList();

            foreach (var user in mapped.Items)
            {
                user.Companies =
                    _mapper.Map<IEnumerable<CompanyWithUserRoleDTO>>(usersCompaniesList.Where(x => x.UserId == user.User.Id));
            }

            return new Response<PageModel<UserWithCompaniesDTO>>
            {
                Data = mapped
            };
        }

        public async Task<Response<UserDetailDTO>> GetUserDetail(Guid userId)
        {
            var userEntity = await _repository.GetById(userId);
            
            if (userEntity == null)
            {
                return new BusinessConflictErrorResponse<UserDetailDTO>(new Error
                {
                    Code = ErrorCodes.Common.NotFound,
                    Message = ErrorCodes.Common.NotFound,
                    Field = ErrorFields.User.Id
                });
            }
            
            var result = _mapper.Map<UserModel, UserDetailDTO>(userEntity);
            return new Response<UserDetailDTO>
            {
                Data = result
            };
        }

        public async Task<Response<UserDetailDTO>> GetUserDetail(string email)
        {
            var userEntity = await _repository.GetByEmail(email);
            
            if (userEntity == null)
            {
                return new BusinessConflictErrorResponse<UserDetailDTO>(new Error
                {
                    Code = ErrorCodes.Common.NotFound,
                    Message = ErrorCodes.Common.NotFound,
                    Field = ErrorFields.User.Id
                });
            }
            
            var result = _mapper.Map<UserModel, UserDetailDTO>(userEntity);
            return new Response<UserDetailDTO>
            {
                Data = result
            };
        }
    }
}