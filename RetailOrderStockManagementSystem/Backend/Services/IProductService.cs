using RetailOrderStockManagementSystem.Backend.DTOs;

namespace RetailOrderStockManagementSystem.Backend.Services;

public interface IProductService
{
    Task<List<ProductResponseAdminDto>> GetAllAsync(ProductQueryParameters parameters);
    Task<ProductResponseAdminDto?> GetByIdAsync(int id);
    Task<ProductResponseAdminDto> CreateAsync(CreateProductRequestDto dto);
    Task<bool> UpdateAsync(UpdateProductRequestDto dto, int id);
    //Task<bool> DeleteAsync(int id);
}
