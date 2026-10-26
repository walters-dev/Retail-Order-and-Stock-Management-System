using RetailOrderStockManagementSystem.Backend.DTOs;

namespace RetailOrderStockManagementSystem.Backend.Services.Interfaces;

public interface IProductService
{
    Task<List<ProductResponseDto>> GetAllAsync();
    Task<ProductResponseDto?> GetByIdAsync(int id);
    Task<ProductResponseDto> CreateAsync(CreateProductRequestDto dto);
    Task<bool> UpdateAsync(int id, UpdateProductRequestDto dto);
    Task<bool> DeleteAsync(int id);
}
