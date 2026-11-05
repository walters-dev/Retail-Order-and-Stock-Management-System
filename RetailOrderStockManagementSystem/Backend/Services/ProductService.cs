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
        string sku = dto.SKU.Trim();

        if (string.IsNullOrEmpty(sku))
            throw new ArgumentException("SKU is required.");

        if (await _repository.SkuExistsAsync(sku))
            throw new ArgumentException($"A product with this sku ({sku}) already exists.");

            var product = new Product
        {
            SKU = sku,
            Name = dto.Name.Trim(),
            Category = dto.Category.Trim(),
            UnitPrice = dto.UnitPrice ?? 0,
            IsActive = true,
            StockQuantity = dto.StockQuantity ?? 0
            //,CreatedAt = dto.CreatedAt,
            //UpdatedAt = dto.UpdatedAt
        };

        product = await _repository.AddAsync(product);
        return MapToDto(product);
    }

    public async Task<bool> UpdateAsync(UpdateProductRequestDto dto, int id)
    {
        var product = await _repository.GetByIdAsync(id);
        string sku = dto.SKU.Trim();

        if (product == null)
            return false;

        if (await _repository.SkuExistsAsync(sku, id))
            throw new ArgumentException($"A product with this sku ({sku}) already exists.");

        product.SKU = sku;
        product.Name = dto.Name.Trim();
        product.Category = dto.Category.Trim();
        product.UnitPrice = dto.UnitPrice;
        product.StockQuantity = dto.StockQuantity;
        product.IsActive = dto.IsActive ?? false;

        product.UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow);


        await _repository.UpdateAsync(id);
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
        ProductId = p.ProductId,
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
