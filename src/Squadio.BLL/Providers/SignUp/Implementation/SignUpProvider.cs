using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapper;
using Squadio.BLL.Providers.Companies;
using Squadio.BLL.Providers.Invites;
using Squadio.BLL.Providers.Teams;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.SignUp;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Users;

namespace Squadio.BLL.Providers.SignUp.Implementation
{
    public class SignUpProvider : ISignUpProvider
    {
        private readonly ISignUpRepository _repository;
        private readonly IInvitesProvider _invitesProvider;
        private readonly ICompaniesProvider _companiesProvider;
        private readonly ITeamsProvider _teamsProvider;
        private readonly IMapper _mapper;
        public SignUpProvider(ISignUpRepository repository
            , IInvitesProvider invitesProvider
            , ICompaniesProvider companiesProvider
            , ITeamsProvider teamsProvider
            , IMapper mapper)
        {
            _repository = repository;
            _invitesProvider = invitesProvider;
            _companiesProvider = companiesProvider;
            _teamsProvider = teamsProvider;
            _mapper = mapper;
        }

        public async Task<Response<SignUpStepDTO>> GetRegistrationStep(string email)
        {
            var entity = await _repository.GetRegistrationStepByEmail(email);
            if (entity == null)
            {
                return new BusinessConflictErrorResponse<SignUpStepDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Business.UserDoesNotExists,
                        Message = ErrorMessages.Business.UserDoesNotExists,
                        Field = ErrorFields.User.Email
                    }
                });
            }
            
            var result = _mapper.Map<UserRegistrationStepModel, SignUpStepDTO>(entity);
            return new Response<SignUpStepDTO>
            {
                Data = result
            };
        }

        public async Task<Response<IEnumerable<string>>> GetTeamInvites(Guid userId)
        {
            var step = await _repository.GetRegistrationStepByUserId(userId);

            if (step.Step >= RegistrationStep.Done)
            {
                return new BusinessConflictErrorResponse<IEnumerable<string>>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Business.InvalidRegistrationStep,
                        Message = ErrorMessages.Business.InvalidRegistrationStep
                    }
                });
            }
            
            var companyPage = await _companiesProvider.GetCompaniesOfUser(userId, new PageModel());
            var company = companyPage.Data.Items.FirstOrDefault();

            var teamPage = await _teamsProvider.GetTeams(new PageModel(), company.Id);
            var team = teamPage.Data.Items.FirstOrDefault();
            
            var inviteResponse = await _invitesProvider.GetTeamInvites(team.Id);
            return new Response<IEnumerable<string>>
            {
                Data = inviteResponse.Data.Select(x => x.Email).Distinct()
            };
        }
    }
}