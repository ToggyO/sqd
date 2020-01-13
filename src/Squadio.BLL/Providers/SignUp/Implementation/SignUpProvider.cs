﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapper;
using Squadio.BLL.Providers.Companies;
using Squadio.BLL.Providers.Teams;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Invites;
using Squadio.DAL.Repository.SignUp;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Invites;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Invites;
using Squadio.DTO.SignUp;

namespace Squadio.BLL.Providers.SignUp.Implementation
{
    public class SignUpProvider : ISignUpProvider
    {
        private readonly ISignUpRepository _repository;
        private readonly IInvitesRepository _invitesRepository;
        private readonly ICompaniesProvider _companiesProvider;
        private readonly ITeamsProvider _teamsProvider;
        private readonly IMapper _mapper;
        public SignUpProvider(ISignUpRepository repository
            , IInvitesRepository invitesRepository
            , ICompaniesProvider companiesProvider
            , ITeamsProvider teamsProvider
            , IMapper mapper)
        {
            _repository = repository;
            _invitesRepository = invitesRepository;
            _companiesProvider = companiesProvider;
            _teamsProvider = teamsProvider;
            _mapper = mapper;
        }

        public async Task<Response<SignUpStepDTO>> GetRegistrationStep(Guid userId)
        {
            var entity = await _repository.GetRegistrationStepByUserId(userId);
            if (entity == null)
            {
                return new BusinessConflictErrorResponse<SignUpStepDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Business.UserDoesNotExists,
                        Message = ErrorMessages.Business.UserDoesNotExists,
                        Field = ErrorFields.User.Token
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

            if (step.Status != UserStatus.Admin)
            {
                return new PermissionDeniedErrorResponse<IEnumerable<string>>(new Error
                {
                    Code = ErrorCodes.Security.PermissionDenied,
                    Message = ErrorMessages.Security.PermissionDenied
                });
            }
            
            var companyPage = await _companiesProvider.GetUserCompanies(userId, new PageModel());
            var company = companyPage.Data.Items.FirstOrDefault();
            
            if (company == null)
            {
                return new BusinessConflictErrorResponse<IEnumerable<string>>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Common.NotFound,
                        Message = ErrorMessages.Common.NotFound,
                        Field = "companyId"
                    }
                });
            }

            var teamPage = await _teamsProvider.GetTeams(new PageModel(), company.CompanyId);
            var team = teamPage.Data.Items.FirstOrDefault();
            
            if (team == null)
            {
                return new BusinessConflictErrorResponse<IEnumerable<string>>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Common.NotFound,
                        Message = ErrorMessages.Common.NotFound,
                        Field = "teamId"
                    }
                });
            }
            
            var inviteResponse = await GetInvitesByEntityId(team.Id);
            return new Response<IEnumerable<string>>
            {
                Data = inviteResponse.Data.Select(x => x.Email).Distinct()
            };
        }
        
        private async Task<Response<IEnumerable<InviteDTO>>> GetInvitesByEntityId(Guid entityId, bool? activated = null)
        {
            var invites = await _invitesRepository.GetInvites(
                entityId: entityId, 
                activated: activated);
            return new Response<IEnumerable<InviteDTO>>
            {
                Data = invites.Select(x => _mapper.Map<InviteModel, InviteDTO>(x))
            };
        }
    }
}