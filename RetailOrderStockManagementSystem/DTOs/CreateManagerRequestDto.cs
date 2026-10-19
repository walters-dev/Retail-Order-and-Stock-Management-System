using System.ComponentModel.DataAnnotations;

namespace RetailOrderStockManagementSystem.DTOs
{
    public class CreateManagerRequestDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Must be a valid email")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        public string Email { get; set; } = "";
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(20, ErrorMessage = "Username cannot exceed 20 characters.")]
        public string Username { get; set; } = "";
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(30, ErrorMessage = "Password cannot exceed 30 characters.")]
        public string Password { get; set; } = "";
    }
}
