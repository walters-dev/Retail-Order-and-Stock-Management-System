using RetailOrderStockManagementSystem.Backend.Enums;

namespace RetailOrderStockManagementSystem.Backend.DTOs
{
    /// <summary>
    /// Provides the financial, status, and identity fields returned to administrators for an order.
    /// </summary>
    public class OrderResponseAdminDto
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public DateOnly OrderDate { get; set; }
        public decimal Subtotal { get; set; }
        public int? DiscountRuleId { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalTotal { get; set; }
        public OrderStatus.Status Status { get; set; }
        public DateOnly CreatedAt { get; set; }
        public DateOnly? UpdatedAt { get; set; }
    }
}
