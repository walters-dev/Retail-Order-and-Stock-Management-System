namespace RetailOrderStockManagementSystem.Backend.Pricing_and_Discount;

public sealed class OrderPricingResult
{
    public decimal Subtotal { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalTotal { get; set; }
    public int? DiscountRuleId { get; set; }
}
