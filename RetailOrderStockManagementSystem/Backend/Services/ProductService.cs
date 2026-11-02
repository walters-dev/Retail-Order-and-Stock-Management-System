using Microsoft.IdentityModel.Tokens;
using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Models;
using RetailOrderStockManagementSystem.Backend.Repositories;

namespace RetailOrderStockManagementSystem.Backend.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ProductResponseAdminDto>> GetAllAsync(ProductQueryParameters parameters)
    {
        var products = await _repository.GetAllAsync(parameters);
        return products.Select(MapToDto).ToList();
    }

    public async Task<ProductResponseAdminDto?> GetByIdAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        return product == null ? null : MapToDto(product);
    }

    public async Task<ProductResponseAdminDto> CreateAsync(CreateProductRequestDto dto)
    {
        if (string.IsNullOrEmpty(dto.SKU))
            throw new ArgumentException("SKU is required.");
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Name is required.");
        if (dto.UnitPrice <= 0)
            throw new ArgumentException("Unit price must be greater than zero.");
        if (dto.StockQuantity < 0)
            throw new ArgumentException("Stock quantity may not be negative.");

        var product = new Product
        {
            SKU = dto.SKU,
            Name = dto.Name,
            Category = dto.Category,
            UnitPrice = dto.UnitPrice,
            StockQuantity = dto.StockQuantity
        };

        product = await _repository.AddAsync(product);
        return MapToDto(product);
    }

    public async Task<bool> UpdateAsync(int id, UpdateProductRequestDto dto)
    {
        var product = await _repository.GetByIdAsync(id);

        if (product == null)
            return false;

        product.SKU = dto.SKU;
        product.Name = dto.Name;
        product.Category = dto.Category;
        product.UnitPrice = dto.UnitPrice;
        product.StockQuantity = dto.StockQuantity;

        await _repository.UpdateAsync(product);
        return true;
    }

    //public async Task<bool> DeleteAsync(int id)
    //{
    //    var product = await _repository.GetByIdAsync(id);

    //    if (product == null)
    //        return false;

    //    await _repository.DeleteAsync(product);
    //    return true;
    //}

    private static ProductResponseAdminDto MapToDto(Product p) => new()
    {
        SKU = p.SKU,
        Name = p.Name,
        Category = p.Category,
        UnitPrice = p.UnitPrice,
        StockQuantity = p.StockQuantity,
        IsActive = p.IsActive,
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt
    };
}
