namespace RetailOrderStockManagementSystem.Backend.DTOs
{
    /// <summary>
    /// Provides the full product representation returned to administrative users.
    /// </summary>
    public class ProductResponseAdminDto
    {
        public int ProductId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
        public DateOnly CreatedAt { get; set; }
        public DateOnly? UpdatedAt { get; set; }
    }
}
