using RetailOrderStockManagementSystem.Backend.Data;
using RetailOrderStockManagementSystem.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace RetailOrderStockManagementSystem.Backend.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly RetailDbContext _context;
        public AuthRepository(RetailDbContext context)
        {
            _context = context;
        }

        public async Task<User> AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<bool> usernameExist(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }
        public async Task<bool> emailExist(string email)
        {
            return await _context.Users.AnyAsync(e => e.Email == email);
        }
        public async Task<User?> GetbyUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}
