namespace RetailOrderStockManagementSystem.Backend.Models
{
    /// <summary>
    /// Represents one product and quantity within an order, preserving the price used at purchase time.
    /// </summary>
    public class OrderLineItem
    {
        public int OrderLineItemId { get; set; }
        // ProductId and OrderId are the database foreign keys. They identify the related rows
        // without requiring the related objects to be loaded.
        public int ProductId { get; set; }
        public Guid OrderId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
        // Product is the navigation property for reading product details, such as the name,
        // when an order response includes related product information.
        public Product? Product { get; set; }

        // Order is the inverse navigation property used by EF Core to connect this line back
        // to its parent order. The OrderId foreign key remains the persisted relationship key.
        public Order? Order { get; set; }
    }
}
