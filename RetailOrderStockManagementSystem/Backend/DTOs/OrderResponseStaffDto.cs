using RetailOrderStockManagementSystem.Backend.Enums;

namespace RetailOrderStockManagementSystem.Backend.DTOs
{
    public class OrderResponseStaffDto
    {
        public DateOnly OrderDate { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalTotal { get; set; }
        public OrderStatus.Status Status { get; set; }
    }
}
