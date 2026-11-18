namespace RetailOrderStockManagementSystem.Backend.Pricing_and_Discount;

/// <summary>
/// Contains the calculated monetary totals and the discount rule selected for an order.
/// </summary>
public sealed class OrderPricingResult
{
    public decimal Subtotal { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalTotal { get; set; }
    public int? DiscountRuleId { get; set; }
}
