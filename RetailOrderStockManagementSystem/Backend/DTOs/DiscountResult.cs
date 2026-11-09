namespace RetailOrderStockManagementSystem.Backend.DTOs;

public class DiscountResult
{
    public string DiscountName { get; set; } = string.Empty;
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public int? DiscountRuleId { get; set; }
}
