namespace RetailOrderStockManagementSystem.Backend.DTOs;

/// <summary>
/// Describes one calculated order line in a quote.
/// </summary>
public sealed class OrderLineQuoteDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}
