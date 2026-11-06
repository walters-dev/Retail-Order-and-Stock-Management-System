using System.ComponentModel.DataAnnotations;

namespace RetailOrderStockManagementSystem.Backend.DTOs;

public class CustomerQueryParameters
{
    [StringLength(255)]
    public string? Search { get; set; }

    public bool? IsActive { get; set; }

    [Range(1, int.MaxValue,
        ErrorMessage = "Page must be at least 1.")]
    public int Page { get; set; } = 1;

    [Range(1, 100,
        ErrorMessage = "Page size must be between 1 and 100.")]
    public int PageSize { get; set; } = 10;
}