namespace RetailOrderStockManagementSystem.Backend.Models
{
    public class Customer
    {
        public Guid CustomerId {  get; set; }
        public string FullName { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string? Email {  get; set; }
        public bool IsActive { get; set; }
        public bool FirstTimeCustomer { get; set; } = true;
        public bool LoyaltyCustomer { get; set; } = false;
        public DateOnly CreatedAt { get; set; }
        public DateOnly? UpdatedAt { get; set; }
    }
}
