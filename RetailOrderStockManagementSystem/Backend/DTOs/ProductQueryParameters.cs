using System.ComponentModel.DataAnnotations;

namespace RetailOrderStockManagementSystem.Backend.DTOs;

public class ProductQueryParameters
{
    public string? Search { get; set; }
    public string? SortBy { get; set; } 
    public bool Descending { get; set; }
    public bool IncludeInactive { get; set; }

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 50)]
    public int PageSize { get; set; } = 10;
}
