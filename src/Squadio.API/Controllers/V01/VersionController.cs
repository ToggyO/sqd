using System;
using System.Threading.Tasks;
using Magora.Passwords;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Companies;
using Squadio.DAL.Repository.SignUp;
using Squadio.DAL.Repository.Users;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Companies;
using Squadio.Domain.Models.Users;

namespace Squadio.API.Controllers.V01
{
    [ApiController]
    [AllowAnonymous]
    [ApiVersion("0.1")]
    [Route("v{version:apiVersion}/versions")]
    public class VersionController : ControllerBase
    {
        private const string Version = "0.0.1 b";
        private readonly ILogger<VersionController> _logger;
        // private readonly ICompaniesRepository _companiesRepository;
        // private readonly IUsersRepository _usersRepository;
        // private readonly ISignUpRepository _signUpRepository;
        // private readonly IPasswordService _passwordService;

        public VersionController(ILogger<VersionController> logger
            // , ICompaniesRepository companiesRepository
            // , IUsersRepository usersRepository
            // , ISignUpRepository signUpRepository
            // , IPasswordService passwordService
            )
        {
            _logger = logger;
            // _companiesRepository = companiesRepository;
            // _usersRepository = usersRepository;
            // _signUpRepository = signUpRepository;
            // _passwordService = passwordService;
        }

        /// <summary>
        /// Get current version of API
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public Response<string> GetVersion()
        {
            return new Response<string>
            {
                Data = Version
            };
        }

        // [HttpGet("CreateUsers")]
        // [Obsolete]
        // [AllowAnonymous]
        // public async Task<Response> CreateUsers()
        // {
        //     var userNumber = 1;
        //     var password = "123456";
        //     var passwordModel = await _passwordService.CreatePassword(password);
        //     while (userNumber <= 15)
        //     {
        //         var name = $"farshatov+{userNumber}";
        //         var email = $"farshatov+{userNumber}@magora-systems.com";
        //         var exist = await _usersRepository.GetByEmail(email);
        //         if (exist == null)
        //         {
        //             var userEntity = await _usersRepository.Create(new UserModel
        //             {
        //                 CreatedDate = DateTime.Now,
        //                 Email = email,
        //                 Hash = passwordModel.Hash,
        //                 Salt = passwordModel.Salt,
        //                 RoleId = RoleGuid.User,
        //                 IsDeleted = false,
        //                 Name = name,
        //                 Status = UserStatus.Active
        //             });
        //             await _signUpRepository.SetRegistrationStep(userEntity.Id, RegistrationStep.Done,
        //                 MembershipStatus.Member);
        //         }
        //         userNumber++;
        //     }
        //
        //     return new Response();
        // }

        // [HttpGet("CreateCompanies")]
        // [Obsolete]
        // [AllowAnonymous]
        // public async Task<Response> CreateCompanies()
        // {
        //     var companyNumber = 0;
        //     var cityNumber = 0;
        //     while (companyNumber < 25)
        //     {
        //         if (companyNumber % 5 == 0)
        //         {
        //             cityNumber++;
        //         }
        //         companyNumber++;
        //         await _companiesRepository.Create(
        //             new CompanyModel
        //             {
        //                 Address = $"City {cityNumber}",
        //                 Name = $"Company {companyNumber}",
        //                 CreatedDate = DateTime.Now,
        //                 CreatorId = Guid.Parse("6681fcbd-219b-4740-b6ca-29d0bd3572df"),
        //                 IsDeleted = false,
        //                 Status = CompanyStatus.Active
        //             });
        //         Console.WriteLine($"Comp: {companyNumber} || City: {cityNumber}");
        //     }
        //
        //     return new Response();
        // }
    }
}