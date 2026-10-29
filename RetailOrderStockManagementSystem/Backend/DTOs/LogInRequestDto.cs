using System.ComponentModel.DataAnnotations;

namespace RetailOrderStockManagementSystem.Backend.DTOs
{
    public class LogInRequestDto
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(20, ErrorMessage = "Username cannot exceed 20 characters.")]
        public string Username { get; set; } = "";
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(30, ErrorMessage = "Password cannot exceed 30 characters.")]
        public string Password { get; set; } = "";
    }
}
