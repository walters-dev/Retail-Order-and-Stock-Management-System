using System.ComponentModel.DataAnnotations;

namespace RetailOrderStockManagementSystem.DTOs
{
    public class CreateProductResponseDto
    {
        [Required(ErrorMessage = "SKU is required.")]
        public string SKU { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Category is required.")]
        public string Category { get; set; }
        [Required(ErrorMessage = "UnitPrice is required.")]
        public decimal UnitPrice { get; set; }
        [Required(ErrorMessage = "StockQuantity is required.")]
        [Range(0, 9999, ErrorMessage = "Must be .")]
        public int StockQuantity { get; set; }
    }
}
