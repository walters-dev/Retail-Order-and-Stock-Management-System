using AutoMapper;
using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Models;
using RetailOrderStockManagementSystem.Backend.Repositories;
using static QRCoder.PayloadGenerator;

namespace RetailOrderStockManagementSystem.Backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        public AuthService(IAuthRepository repository, IMapper mapper, ILogger<AuthService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<LogInRequestDto> CreateUserAsync(CreateUserRequestDto dto)
        {
            var user = _mapper.Map<User>(dto);
            var repoUser =  await _repository.AddUserAsync(user);
            _logger.LogInformation("Created user {UserId} {UserUsername}", repoUser.UserId, repoUser.Username);
            return _mapper.Map<LogInRequestDto>(repoUser);
        }
        public async Task<bool> usernameExist(string username)
        {
            bool user = await _repository.usernameExist(username);
            if (user == false)
            {
                return false;
            }
            _logger.LogWarning("User {UserUsername} already exists", username);
            return true;
        }
        public async Task<bool> emailExist(string email)
        {
            var user = await _repository.emailExist(email);
            if (user == false)
            {
                return false;
            }
            _logger.LogWarning("User {UserEmail} already exists", email);
            return true;
        }
        public async Task<LogInRequestDto?> GetbyUsernameAsync(string username)
        {
            _logger.LogInformation("Retrieving user {UserUsername}", username);
            var user = await _repository.GetbyUsernameAsync(username);
            if (user == null)
            {
                _logger.LogWarning("User {UserUsername} not found", username);
                return null;
            }
            return _mapper.Map<LogInRequestDto>(user);
        }
    }
}
