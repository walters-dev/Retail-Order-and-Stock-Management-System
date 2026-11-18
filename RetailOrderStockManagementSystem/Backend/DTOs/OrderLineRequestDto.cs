namespace RetailOrderStockManagementSystem.Backend.DTOs;

/// <summary>
/// Identifies a product and requested quantity when creating or quoting an order.
/// </summary>
public sealed class OrderLineRequestDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
