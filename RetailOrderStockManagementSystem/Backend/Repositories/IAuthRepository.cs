using RetailOrderStockManagementSystem.Backend.Data;
using RetailOrderStockManagementSystem.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace RetailOrderStockManagementSystem.Backend.Repositories
{
    public interface IAuthRepository
    {
        public Task<User> AddUserAsync(User user);
        public Task<bool> usernameExist(string username);
        public Task<bool> emailExist(string email);
        public Task<User?> GetbyUsernameAsync(string username);
    }
}
