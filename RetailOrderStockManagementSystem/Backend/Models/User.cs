using RetailOrderStockManagementSystem.Backend.Enums;

namespace RetailOrderStockManagementSystem.Backend.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Users.UserTypes Type { get; set; }
    }
}
