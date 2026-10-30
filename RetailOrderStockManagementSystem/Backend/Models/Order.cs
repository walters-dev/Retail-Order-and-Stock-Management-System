using RetailOrderStockManagementSystem.Backend.Enums;

namespace RetailOrderStockManagementSystem.Backend.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public DateOnly OrderDate { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalTotal { get; set; }
        public OrderStatus.Status Status { get; set; }
        public DateOnly CreatedAt { get; set; }
        public DateOnly? UpdatedAt { get; set; }
        public int? DiscountRuleId { get; set; }
        public int ReceiptId { get; set; }
    }
}
