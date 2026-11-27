using System.ComponentModel.DataAnnotations;

namespace RetailOrderStockManagementSystem.Backend.DTOs
{
    public class CreateCustomerRequestDto
    {
        [Required(ErrorMessage = "Full Name is required.")]
        [RegularExpression(@"^[A-Za-z][A-Za-z\'\-]+([ A-Za-z][A-Za-z\'\-]+)*$", ErrorMessage = "Full name must not have symbols or numbers.")]//allows spaces, apostrphes and hyphens
        [StringLength(255, ErrorMessage = "FullName cannot exceed 255 characters.")]
        public string FullName { get; set; } = "";
        
        [Required(ErrorMessage = "Mobile Number is required.")]
        [Phone(ErrorMessage = "Phone number must be valid")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "Must be a valid South African Mobile Number e.g. 0824890524 or +27 824890524")]
        public string MobileNumber { get; set; } = "";

        [RegularExpression(@"^\s*(?:[A-Za-z0-9_%+\-]+(?:\.[A-Za-z0-9_%+\-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9\-]*[A-Za-z0-9])?\.)+[A-Za-z]{2,})?\s*$",
        ErrorMessage = "Enter a valid email address.")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        public string? Email { get; set; }
        //IsActive, CreatedAt, UpdatedAt isnt user inputted so do i need to make this required/validated
    }
}
