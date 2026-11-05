using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using RetailOrderStockManagementSystem.Backend.Data;
using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Enums;
using RetailOrderStockManagementSystem.Backend.Models;
using RetailOrderStockManagementSystem.Backend.Security;
using RetailOrderStockManagementSystem.Backend.Services;

namespace RetailOrderStockManagementSystem.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;
        private readonly JwtTokenGenerator _jwtTokenGenerator;
        private readonly IMapper _mapper;
        public AuthController(IAuthService service, JwtTokenGenerator jwtTokenGenerator, IMapper mapper)
        {
            _service = service;
            _jwtTokenGenerator = jwtTokenGenerator;
            _mapper = mapper;
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
            dto.Username = dto.Username.Trim();
            dto.Password = dto.Password.Trim();
            dto.Email = dto.Email.Trim();
            //check username exist service
            bool usernameExists = await _service.usernameExist(dto.Username);
            if (usernameExists)
            {
                return Conflict(new ApiErrorResponseDto
                {
                    Error = "Conflict",
                    Message = "Username already exists."
                });
            }
            //check email exist service
            bool emailExists = await _service.emailExist(dto.Email);
            if (emailExists)
            {
                return Conflict(new ApiErrorResponseDto
                {
                    Error = "Conflict",
                    Message = "Email already exists."
                });
            }
            Encryption encryption = new Encryption();
            dto.Password = encryption.HashPassword(dto.Password);

            //save service here
            await _service.CreateUserAsync(dto);
            return StatusCode(201,new {message = "User created successfully."});//CreatedAt doesnt work bcs you need AdminbyId
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LogInRequestDto dto)
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
            dto.Username = dto.Username.Trim();
            dto.Password = dto.Password.Trim();
            //find by username
            var user = await _service.GetbyUsernameAsync(dto.Username);
            if (user == null)
            {
                return Unauthorized(new ApiErrorResponseDto
                {
                    Error = "Unauthorized",
                    Message = "Invalid username or password"
                });
            }
            Encryption encryption = new Encryption();
            bool loggedin = encryption.VerifyPassword(dto.Password, user.Password);
            if (!loggedin)
            {
                return Unauthorized(new ApiErrorResponseDto
                {
                    Error = "Unauthorized",
                    Message = "Invalid username or password"
                });
            }
            var newuser = _mapper.Map<User>(user);
            string token = _jwtTokenGenerator.GenerateToken(newuser);
            return Ok(new
            {
                token,
                expiresInSeconds = 3600,
                role = user.Type
            });
        }
    }
}
