using System.ComponentModel.DataAnnotations;

namespace RetailOrderStockManagementSystem.Backend.DTOs
{
    public class CreateProductRequestDto
    {
        [Required(ErrorMessage = "SKU is required.")]
        [StringLength(8, ErrorMessage = "SKU must be 8 characters long.", MinimumLength = 8)]
        [RegularExpression(@"^[A-Za-z]{3}[0-9]{3}[A-Za-z]{2}", ErrorMessage = "Invalid SKU code.")]//checks if it follows the sku format
        public string SKU { get; set; } = string.Empty;//need regex check
        
        [Required(ErrorMessage = "Name is required.")]
        [RegularExpression(@"^[A-Za-z][A-Za-z\'\-]+([ A-Za-z][A-Za-z\'\-]+)*$", ErrorMessage = "Name must not have symbols or numbers.")]//allows spaces, apostrophes and hyphens
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Category is required.")]
        [RegularExpression(@"^[A-Za-z][A-Za-z\'\-]+([ A-Za-z][A-Za-z\'\-]+)*$", ErrorMessage = "Category must not have symbols or numbers.")]//allows spaces, apostrophes and hyphens
        public string Category { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "UnitPrice is required.")]
        [RegularExpression(@"^(((\d{1,2})(,\d{2})*)|(\d+))(.\d+)?$", ErrorMessage = "UnitPrice must be a valid ")]//checks if its a decimal
        [Range(0.01, 9999.99, ErrorMessage = "Unit Price must be within the range of 0,01-9999,99.")]
        public decimal UnitPrice { get; set; }
        
        [Required(ErrorMessage = "StockQuantity is required.")]
        [RegularExpression("^[0-9]+$", ErrorMessage ="Stock Quantity must be a whole number.")]
        [Range(0, 9999, ErrorMessage = "Stock quantity must be within the range of 0-9999.")]
        public int StockQuantity { get; set; }
    }
}
