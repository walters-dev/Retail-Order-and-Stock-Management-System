namespace RetailOrderStockManagementSystem.Backend.DTOs
{
    /// <summary>
    /// Provides the product fields available to staff without administrative metadata.
    /// </summary>
    public class ProductResponseStaffDto
    {
        public int ProductId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
    }
}
