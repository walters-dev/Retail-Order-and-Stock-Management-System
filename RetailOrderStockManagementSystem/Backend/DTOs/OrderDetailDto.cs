namespace RetailOrderStockManagementSystem.Backend.DTOs;

/// <summary>
/// Provides the complete order view, including customer and line details.
/// </summary>
public sealed class OrderDetailDto : OrderResponseAdminDto
{
    public CustomerOrderDto? Customer { get; set; }
    public List<OrderLineDetailDto> Items { get; set; } = new();
}
