using RetailOrderStockManagementSystem.Backend.Enums;
using RetailOrderStockManagementSystem.Backend.Migrations;

namespace RetailOrderStockManagementSystem.Backend.DTOs
{
    /// <summary>
    /// Provides the order fields exposed to staff while limiting administrative details.
    /// </summary>
    public class OrderResponseStaffDto
    {
        public Guid OrderId { get; set; }
        public int ReceiptId { get; set; }
        public Guid CustomerId { get; set; }
        public DateOnly OrderDate { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalTotal { get; set; }
        public OrderStatus.Status Status { get; set; }
    }
}
