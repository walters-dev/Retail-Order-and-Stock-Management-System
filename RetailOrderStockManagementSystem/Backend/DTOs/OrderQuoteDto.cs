namespace RetailOrderStockManagementSystem.Backend.DTOs;

/// <summary>
/// Returns calculated pricing without creating an order.
/// </summary>
public sealed class OrderQuoteDto
{
    public Guid CustomerId { get; set; }
    public List<OrderLineQuoteDto> Items { get; set; } = new();
    public int? DiscountRuleId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalTotal { get; set; }
}
