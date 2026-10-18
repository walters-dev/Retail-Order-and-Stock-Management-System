using System.ComponentModel.DataAnnotations;

namespace RetailOrderStockManagementSystem.DTOs
{
    public class CreateCustomerResponseDto
    {
        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(255, ErrorMessage = "FullName cannot exceed 255 characters.")]
        public string FullName { get; set; } = "";
        [Required(ErrorMessage = "Mobile Number is required.")]
        [Phone(ErrorMessage = "Phone number must be valid")]
        public string MobileNumber { get; set; } = "";
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Must be a valid email")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        public string Email { get; set; } = "";
        //IsActive, CreatedAt, UpdatedAt isnt user inputted so do i need to make this required/validated
    }
}
