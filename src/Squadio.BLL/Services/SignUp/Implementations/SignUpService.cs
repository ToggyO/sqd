using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Squadio.BLL.Providers.Users;
using Squadio.BLL.Services.Users;
using Squadio.Common.Models.Responses;
using Squadio.DTO.SignUp;
using Squadio.DTO.Users;

namespace Squadio.BLL.Services.SignUp.Implementations
{
    public class SignUpService : ISignUpService
    {
        private readonly IUsersService _usersService;
        private readonly IUsersProvider _usersProvider;
        private readonly IMapper _mapper;
        private readonly ILogger<SignUpService> _logger;

        public SignUpService(IUsersService usersService
            , IUsersProvider usersProvider
            , IMapper mapper
            , ILogger<SignUpService> logger)
        {
            _usersService = usersService;
            _usersProvider = usersProvider;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<Response> SignUp(SignUpSimpleDTO dto)
        {
            var createDto = _mapper.Map<UserCreateDTO>(dto);
            await _usersService.CreateUser(createDto);
            await _usersService.SetPassword(dto.Email, dto.Password);
            return new Response();
        }
    }
}