namespace RetailOrderStockManagementSystem.Backend.Models
{
    /// <summary>
    /// Defines a discount percentage and the minimum order amount used by applicable rules.
    /// </summary>
    public class DiscountRule
    {
        public int DiscountRuleId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Percent { get; set; }
        public decimal MinimumOrderAmount { get; set; }
        public bool IsActive { get; set; }
    }
}
