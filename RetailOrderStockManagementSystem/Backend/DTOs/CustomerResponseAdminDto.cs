namespace RetailOrderStockManagementSystem.Backend.DTOs
{
    public class CustomerResponseAdminDto
    {
        public string FullName { get; set; }
        public string MobileNumber { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        public DateOnly CreatedAt { get; set; }
        public DateOnly? UpdatedAt { get; set; }
    }
}
