namespace RetailOrderStockManagementSystem.Backend.DTOs;

/// <summary>
/// Provides product and pricing details for an order line response.
/// </summary>
public sealed class OrderLineDetailDto
{
    public int OrderLineItemId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}
