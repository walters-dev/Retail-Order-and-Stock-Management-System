using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Models;
using RetailOrderStockManagementSystem.Backend.Services.Interfaces;
using RetailOrderStockManagementSystem.Backend.Repositories;


namespace RetailOrderStockManagementSystem.Backend.Services;

public class ProductService : IProductService
{
    private static readonly List<Product> _products = new()
    {
        new Product { ProductId = 1, SKU = "LPT223HP", Name = "Laptop",
            Category = "Computers", UnitPrice = 12999.99m, StockQuantity = 10,
            IsActive = true, CreatedAt = new DateOnly(2026, 10, 23), UpdatedAt = null},
        new Product { ProductId = 2, SKU = "WMS225HP" , Name = "Wireless Mouse",
            Category = "Accessories", UnitPrice = 349.99m, StockQuantity = 25,
            IsActive = true, CreatedAt = new DateOnly(2026, 10, 23), UpdatedAt = null}
    };


    public async Task<List<ProductResponseDto>> GetAllAsync()
    {
        var products = await Task.FromResult(_products);
        return products.Select(MapToDto).ToList();
    }

    public async Task<ProductResponseDto?> GetByIdAsync(int id)
    {
        var product = await Task.FromResult(_products.FirstOrDefault
            (p => p.ProductId == id));
        return product == null ? null : MapToDto(product);
    }

    public async Task<ProductResponseDto> CreateAsync(CreateProductRequestDto dto)
    {
        int nextId = _products.Any() ? _products.Max(p => p.ProductId) + 1 : 1;

        var newProduct = new Product
        {
            SKU = dto.SKU,
            Name = dto.Name,
            Category = dto.Category,
            UnitPrice = dto.UnitPrice,
            StockQuantity = dto.StockQuantity,
            IsActive = true,
            CreatedAt = DateOnly.FromDateTime(DateTime.Now),
            UpdatedAt = null
        };

        _products.Add(newProduct);
        return await Task.FromResult(MapToDto(newProduct));
    }

    public async Task<bool> UpdateAsync(int id, UpdateProductRequestDto dto)
    {
        var product = await Task.FromResult(_products.FirstOrDefault(p => p.ProductId == id));
        if (product == null) return false;
        
        product.SKU =  dto.SKU;
        product.Name = dto.Name;
        product.Category = dto.Category;
        product.UnitPrice = dto.UnitPrice;
        product.StockQuantity = dto.StockQuantity;
        product.IsActive = true;
        product.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await Task.FromResult(_products.FirstOrDefault(p => p.ProductId == id));
        if (product == null) return false;

        _products.Remove(product);

        return true;
    }

    private ProductResponseDto MapToDto(Product product)
    {
        return new ProductResponseDto
        {
            ProductId = product.ProductId,
            SKU = product.SKU,
            Name = product.Name,
            Category = product.Category,
            UnitPrice = product.UnitPrice,
            StockQuantity = product.StockQuantity,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}
