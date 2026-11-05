using RetailOrderStockManagementSystem.Backend.DTOs;

namespace RetailOrderStockManagementSystem.Backend.Services
{
    public interface IAuthService
    {
        public Task<LogInRequestDto> CreateUserAsync(CreateUserRequestDto user);
        public Task<bool> usernameExist(string username);
        public Task<bool> emailExist(string email);
        public Task<LogInRequestDto?> GetbyUsernameAsync(string username);
    }
}
