using System;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Squadio.Common.Models.Email;
using Squadio.Common.Models.Responses;

namespace Squadio.API.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/versions")]
    public class VersionController : ControllerBase
    {
        private const string Version = "0.1.7 b";
        private readonly ILogger<VersionController> _logger;

        public VersionController(ILogger<VersionController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get current version of API
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public string GetVersion()
        {
            return Version;
        }

        /// <summary>
        /// Push UserConfirm message to RabbitMQ
        /// </summary>
        [HttpPost("UC")]
        [AllowAnonymous]
        public Response PushTestRabbitUC()
        {
            using (var bus = RabbitHutch.CreateBus("host=rabbit-dev;username=rabbitmq;password=rabbitmq"))
            {
                bus.Publish(new UserConfirmEmailModel
                {
                    Code = "WOW!_It's_alive!",
                    To = "karpov@magora-systems.com"
                });
            }

            return new Response();
        }

        /// <summary>
        /// Push PasswordRestore message to RabbitMQ
        /// </summary>
        [HttpPost("PR")]
        [AllowAnonymous]
        public Response PushTestRabbitPR()
        {
            using (var bus = RabbitHutch.CreateBus("host=rabbit-dev;username=rabbitmq;password=rabbitmq"))
            {
                bus.Publish(new PasswordRestoreEmailModel()
                {
                    Code = "WOW!_It's_alive!",
                    To = "karpov@magora-systems.com"
                });
            }

            return new Response();
        }

        /// <summary>
        /// Push InviteToCompany message to RabbitMQ
        /// </summary>
        [HttpPost("IC")]
        [AllowAnonymous]
        public Response PushTestRabbitIC()
        {
            using (var bus = RabbitHutch.CreateBus("host=rabbit-dev;username=rabbitmq;password=rabbitmq"))
            {
                bus.Publish(new InviteToCompanyEmailModel()
                {
                    Code = "WOW!_It's_alive!",
                    To = "karpov@magora-systems.com",
                    AuthorName = "ALICA",
                    CompanyName = "MAGORA",
                    IsAlreadyRegistered = true
                });
            }

            return new Response();
        }

        /// <summary>
        /// Push InviteToTeam message to RabbitMQ
        /// </summary>
        [HttpPost("IT")]
        [AllowAnonymous]
        public Response PushTestRabbitIT()
        {
            using (var bus = RabbitHutch.CreateBus("host=rabbit-dev;username=rabbitmq;password=rabbitmq"))
            {
                bus.Publish(new InviteToTeamEmailModel()
                {
                    Code = "WOW!_It's_alive!",
                    To = "karpov@magora-systems.com",
                    AuthorName = "ALICA",
                    TeamName = ".NET",
                    IsAlreadyRegistered = true
                });
            }

            return new Response();
        }

        /// <summary>
        /// Push InviteToProject message to RabbitMQ
        /// </summary>
        [HttpPost("IP")]
        [AllowAnonymous]
        public Response PushTestRabbitIP()
        {
            using (var bus = RabbitHutch.CreateBus("host=rabbit-dev;username=rabbitmq;password=rabbitmq"))
            {
                bus.Publish(new InviteToProjectEmailModel()
                {
                    Code = "WOW!_It's_alive!",
                    To = "karpov@magora-systems.com",
                    AuthorName = "ALICA",
                    ProjectName = "Squad.IO",
                    IsAlreadyRegistered = true
                });
            }

            return new Response();
        }
    }
}