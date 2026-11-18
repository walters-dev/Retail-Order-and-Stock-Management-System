namespace RetailOrderStockManagementSystem.Backend.Enums
{
    /// <summary>
    /// Defines the allowed lifecycle states for an order.
    /// </summary>
    public class OrderStatus
    {
        public enum Status
        {
            Pending,
            Confirmed,
            Completed,
            Cancelled
        }
    }
}
