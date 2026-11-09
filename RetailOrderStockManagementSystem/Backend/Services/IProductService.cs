using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Models;

namespace RetailOrderStockManagementSystem.Backend.Services;

public interface IProductService
{
    Task<List<ProductResponseAdminDto>> GetAllAsync(ProductQueryParameters parameters);
    Task<List<ProductResponseStaffDto>> GetAllForStaffAsync(ProductQueryParameters parameters);
    Task<ProductResponseAdminDto?> GetByIdAsync(int id);
    Task<ProductResponseStaffDto?> GetByIdForStaffAsync(int id);
    Task<ProductResponseAdminDto> CreateAsync(CreateProductRequestDto dto);
    Task<bool> UpdateAsync(UpdateProductRequestDto dto, int id);
    Task<Product?> UpdateActiveStatusAsync(int id, bool isActive);
}
