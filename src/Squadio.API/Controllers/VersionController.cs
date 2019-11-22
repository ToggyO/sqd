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
        /// Push test message to RabbitMQ
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public Response PushTestRabbit()
        {
            using (var bus = RabbitHutch.CreateBus("host=rabbit-dev;username=rabbitmq;password=rabbitmq"))
            {
                if (bus.IsConnected)
                {
                    try
                    {
                        bus.Publish(new UserConfirmEmailModel
                        {
                            Code = "WOW! It's alive!",
                            To = "azaza@lol.wow"
                        });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        _logger.LogError(e.Message);
                        return new ErrorResponse
                        {
                            Message = e.Message,
                            Code = e.StackTrace
                        };
                    }
                }
                else
                {
                    _logger.LogError("Can't connect to RabbitMQ");
                    return new ErrorResponse
                    {
                        Message = "Can't connect to RabbitMQ",
                    };
                }
            }
            return new Response();
        }
    }
}