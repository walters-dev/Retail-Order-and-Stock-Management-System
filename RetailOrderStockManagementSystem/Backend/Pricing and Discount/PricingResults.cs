namespace RetailOrderStockManagementSystem.Backend.Pricing_and_Discount;

public sealed record PricingResult(
    decimal Subtotal,
    decimal DiscountPercent,
    decimal DiscountAmount,
    decimal FinalTotal);
