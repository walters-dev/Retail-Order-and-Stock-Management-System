namespace RetailOrderStockManagementSystem.Backend.DTOs;

/// <summary>
/// Contains the customer summary included in an order detail response.
/// </summary>
public sealed class CustomerOrderDto
{
    public Guid CustomerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
}
