using RetailOrderStockManagementSystem.Backend.Enums;
using System.Text.Json.Serialization;

namespace RetailOrderStockManagementSystem.Backend.Models
{
    /// <summary>
    /// Represents a customer order, its financial snapshot, status, and related records.
    /// Foreign-key properties identify related rows, while navigation properties let EF Core load or save the object graph.
    /// </summary>
    public class Order
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public DateOnly OrderDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalTotal { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OrderStatus.Status Status { get; set; }
        public DateOnly CreatedAt { get; set; }
        public DateOnly? UpdatedAt { get; set; }
        public int? DiscountRuleId { get; set; }
        public int ReceiptId { get; set; }

        // CustomerId is the database foreign key used to identify the customer row.
        // Customer is the optional navigation property that lets EF Core load the related
        // customer when order details need customer information; it does not replace the FK.
        public Customer? Customer { get; set; }

        // DiscountRuleId stores the applied rule as a foreign key. DiscountRule lets EF Core
        // load the rule details, while remaining nullable when no discount was applied.
        public DiscountRule? DiscountRule { get; set; }

        // This collection is the order-to-lines navigation property. The OrderLineItem.OrderId
        // foreign key identifies the database relationship, while this collection lets EF Core
        // create, load, and map all lines belonging to the order as one object graph.
        public List<OrderLineItem> OrderLineItems { get; set; } = new(); 
    }
}
