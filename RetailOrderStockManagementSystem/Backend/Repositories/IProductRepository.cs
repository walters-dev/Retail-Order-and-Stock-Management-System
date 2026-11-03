using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Models;

namespace RetailOrderStockManagementSystem.Backend.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(ProductQueryParameters parameters);
    Task<Product?> GetByIdAsync(int id);
    Task<Product> AddAsync(Product product);
    Task<bool> UpdateAsync(int id);
    Task<bool> DeleteAsync(int id);
    Task<bool> SkuExistsAsync(string sku, int? excludedProductId = null);
}
