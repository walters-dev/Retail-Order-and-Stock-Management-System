using System.ComponentModel.DataAnnotations;

namespace RetailOrderStockManagementSystem.Backend.DTOs
{
    public class UpdateProductRequestDto
    {
        [Required(ErrorMessage = "SKU is required. Example: ABC123XY.")]
        [StringLength(8, ErrorMessage = "SKU must be exactly 8 characters. Example: ABC123XY.", MinimumLength = 8)]
        [RegularExpression(@"^[A-Za-z]{3}[0-9]{3}[A-Za-z]{2}$", ErrorMessage = "SKU must contain 3 letters, 3 digits, and 2 letters. Example: ABC123XY.")]
        public string SKU { get; set; } = string.Empty; //need regex check
        
        [Required(ErrorMessage = "Product name is required. Example: Wireless Mouse.")]
        [RegularExpression(@"^[A-Za-z][A-Za-z\'\-]+([ A-Za-z][A-Za-z\'\-]+)*$", ErrorMessage = "Product name may contain letters, spaces, apostrophes, or hyphens. Example: Wireless Mouse.")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Category is required. Example: Electronics.")]
        [RegularExpression(@"^[A-Za-z][A-Za-z\'\-]+([ A-Za-z][A-Za-z\'\-]+)*$", ErrorMessage = "Category may contain letters, spaces, apostrophes, or hyphens. Example: Electronics.")]
        public string Category { get; set; } = string.Empty;
        
        [Range(0.01, 9999.99, ErrorMessage = "Unit price must be greater than 0 and no more than 9999.99. Example: 450.00.")]
        public decimal UnitPrice { get; set; }
        
        [Range(0, 9999, ErrorMessage = "Stock quantity must be a whole number from 0 to 9999. Example: 25.")]
        public int StockQuantity { get; set; }

        public bool IsActive { get; set; }
    }
}
