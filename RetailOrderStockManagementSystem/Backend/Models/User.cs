using RetailOrderStockManagementSystem.Backend.Enums;

namespace RetailOrderStockManagementSystem.Backend.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Users.UserTypes Type { get; set; }
    }
}
