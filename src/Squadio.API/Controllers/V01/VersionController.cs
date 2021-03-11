using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Companies;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Companies;

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

        public VersionController(ILogger<VersionController> logger
         // , ICompaniesRepository companiesRepository
         )
        {
            _logger = logger;
            // _companiesRepository = companiesRepository;
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