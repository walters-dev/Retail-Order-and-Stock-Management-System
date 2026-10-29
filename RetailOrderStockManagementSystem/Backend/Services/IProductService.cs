using RetailOrderStockManagementSystem.Backend.DTOs;

namespace RetailOrderStockManagementSystem.Backend.Services;

public interface IProductService
{
    Task<List<ProductResponseAdminDto>> GetAllAsync();
    Task<ProductResponseAdminDto?> GetByIdAsync(int id);
    Task<ProductResponseAdminDto> CreateAsync(CreateProductRequestDto dto);
    Task<bool> UpdateAsync(int id, UpdateProductRequestDto dto);
    Task<bool> DeleteAsync(int id);
}
