namespace RetailOrderStockManagementSystem.Backend.Models
{
    public class OrderLineItem
    {
        public int OrderLineItemId { get; set; }
        public int ProductId { get; set; }
        public Guid OrderId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
        public Product? Product { get; set; }
        public Order? Order { get; set; }
    }
}
