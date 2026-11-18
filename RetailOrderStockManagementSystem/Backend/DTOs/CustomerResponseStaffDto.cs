namespace RetailOrderStockManagementSystem.Backend.DTOs
{
    public class CustomerResponseStaffDto
    {
        public string FullName { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public bool IsActive { get; set; }

    }
}
