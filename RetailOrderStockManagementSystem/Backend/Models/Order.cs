using RetailOrderStockManagementSystem.Backend.Enums;
using System.Text.Json.Serialization;

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
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OrderStatus.Status Status { get; set; }
        public DateOnly CreatedAt { get; set; }
        public DateOnly? UpdatedAt { get; set; }
        public int? DiscountRuleId { get; set; }
        public int ReceiptId { get; set; }
        //public bool LargeOrder { get; set; }

        // navigates EF to return related Customer for order details
        public Customer? Customer { get; set; } 

        // Creates OrderLineItem for each requested Product,
        // then assigns that to the OrderLineItems list
        public List<OrderLineItem> OrderLineItems { get; set; } = new(); 
    }
}
