using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using RetailOrderStockManagementSystem.Backend.Data;
using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Enums;
using RetailOrderStockManagementSystem.Backend.Models;
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
                return BadRequest(new ApiErrorResponseDto
                {
                    Error = "ValidationFailed",
                    Message = "Request validation failed.",
                    Details = ModelState
                });
            }
            string username = dto.Username.Trim();
            string password = dto.Password.Trim();
            string email= dto.Email.Trim();
            bool usernameExists = await _context.Users.AnyAsync(u => u.Username == username);
            if (usernameExists)
            {
                return Conflict(new ApiErrorResponseDto
                {
                    Error = "Conflict",
                    Message = "Username already exists."
                });
            }
            bool emailExists = await _context.Users.AnyAsync(u => u.Username == username);
            if (usernameExists)
            {
                return Conflict(new ApiErrorResponseDto
                {
                    Error = "Conflict",
                    Message = "Email already exists."
                });
            }
            Encryption encryption = new Encryption();
            string hashedPassword = encryption.HashPassword(password);
            Users.UserTypes type;
            switch (dto.Type)
            {
                case "Staff":
                    type = Users.UserTypes.Staff;
                    break;
                case "Admin":
                    type = Users.UserTypes.Admin;
                    break;
                case "Manager":
                    type = Users.UserTypes.Manager;
                    break;
                default:
                    return BadRequest(new ApiErrorResponseDto
                    {
                        Error = "InvalidUserType",
                        Message = "Entered User Type does not exist."
                    });
            }
            var user = new User
            {
                Username = username,
                Email = email,
                Password = hashedPassword,
                Type = type,
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction("User successfully created.");
        }
    }
}
