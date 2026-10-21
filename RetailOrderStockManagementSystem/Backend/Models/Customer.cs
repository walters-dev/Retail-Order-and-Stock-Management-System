namespace RetailOrderStockManagementSystem.Backend.Models
{
    public class Customer
    {
        public Guid CustomerId {  get; set; }
        public string FullName { get; set; }
        public string MobileNumber { get; set; }
        public string? Email {  get; set; }
        public bool IsActive { get; set; }
        public DateOnly CreatedAt { get; set; }
        public DateOnly? UpdatedAt { get; set; }
    }
}
