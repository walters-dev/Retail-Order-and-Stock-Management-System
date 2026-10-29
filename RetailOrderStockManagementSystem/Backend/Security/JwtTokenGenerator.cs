using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RetailOrderStockManagementSystem.Backend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RetailOrderStockManagementSystem.Backend.Security
{
    public class JwtTokenGenerator
    {
        private readonly IConfiguration _configuration;
        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateToken(User user)
        {
            //read settings from secrets.json
            string issuer = _configuration["Jwt:Issuer"]!;
            string audience = _configuration["Jwt:Audience"]!;
            string secret = _configuration["Jwt:Secrets"]!;

            //generating secret key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);//might wanna change this

            //this is identity info in the token
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, Enum.GetName(user.Type)),//idk if this will work, cause its enum but needs to be string
                new Claim("userId", user.UserId.ToString())
            };
            
            //sets a timer to terminate the token i think it needs to be adjusted to internet time
            DateTime expires = DateTime.UtcNow.AddHours(1);

            //creating the token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
