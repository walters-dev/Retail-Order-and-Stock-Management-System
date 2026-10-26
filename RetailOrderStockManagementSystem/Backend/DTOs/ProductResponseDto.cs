namespace RetailOrderStockManagementSystem.Backend.DTOs;

public class ProductResponseDto
{
    public int ProductId { get; set; }
    public string SKU { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public decimal UnitPrice { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
    public DateOnly CreatedAt { get; set; }
    public DateOnly? UpdatedAt { get; set; }
}
