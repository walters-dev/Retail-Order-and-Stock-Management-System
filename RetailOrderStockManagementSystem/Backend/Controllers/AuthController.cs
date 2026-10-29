using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using RetailOrderStockManagementSystem.Backend.Data;
using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Security;

namespace RetailOrderStockManagementSystem.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly RetailDbContext _context;
        private readonly JwtTokenGenerator _jwtTokenGenerator;
        public AuthController(RetailDbContext context, JwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _jwtTokenGenerator = jwtTokenGenerator;
        }
        [HttpPost]
        public async Task<IActionResult> AddAsyncUser(CreateUserRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiErrorResponse)
            }
            string username = dto.Username.Trim();
            string password = dto.Password.Trim();
            bool usernameExists = await _context.Users.AnyAsync(u => u.Username == username);
            if (usernameExists)
            {
                return Conflict(new ApiErrorResponse)
            }
        }
    }
}
