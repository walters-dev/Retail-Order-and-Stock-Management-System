namespace RetailOrderStockManagementSystem.Backend.DTOs;

/// <summary>
/// Carries customer, requested order date, and line-item input from the API to the order workflow.
/// </summary>
public sealed class CreateOrderRequestDto
{
    public Guid CustomerId { get; set; }
    public DateOnly OrderDate { get; set; }
    public List<OrderLineRequestDto> Items { get; set; } = new();
}
