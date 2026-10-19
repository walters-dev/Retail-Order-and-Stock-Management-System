namespace RetailOrderStockManagementSystem.Domain.Models
{
    public class DiscountRule
    {
        public int DiscountRuleId {  get; set; }
        public string Name { get; set; }
        public decimal Percent {  get; set; }
        public decimal MinimumOrderAmount {  get; set; }
        public bool IsActive { get; set; }
    }
}
