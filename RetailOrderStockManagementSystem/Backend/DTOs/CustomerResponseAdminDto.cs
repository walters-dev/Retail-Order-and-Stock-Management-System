namespace RetailOrderStockManagementSystem.Backend.DTOs
{
    public class CustomerResponseAdminDto
    {
        public Guid CustomerId { get; set; } 
        public string FullName { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        public DateOnly CreatedAt { get; set; }
        public DateOnly? UpdatedAt { get; set; }
    }
}
