using RetailOrderStockManagementSystem.Backend.Data;
using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Models;
using RetailOrderStockManagementSystem.Backend.Repositories;
using System.ComponentModel.DataAnnotations;

namespace RetailOrderStockManagementSystem.Backend.Services;

public class ProductService : IProductService
{
    private readonly RetailDbContext _retailDbContext;
    private readonly IProductRepository _repository;

    public ProductService(RetailDbContext retailDbContext, IProductRepository repository)
    {
        _retailDbContext = retailDbContext;
        _repository = repository;
    }

    // GET all products
    public async Task<List<ProductResponseAdminDto>> GetAllAsync(ProductQueryParameters parameters)
    {
        var products = await _repository.GetAllAsync(parameters);

        return products.Select(MapToDto).ToList();
    }

    // GET product ById, returns null if product is not found
    public async Task<ProductResponseAdminDto?> GetByIdAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);

        return product == null ? null : MapToDto(product);
    }

    // POST new product
    public async Task<ProductResponseAdminDto> CreateAsync(CreateProductRequestDto dto)
    {
        // Normalize the SKU before checking uniqueness so casing/whitespace cannot
        // accidentally create two products that should represent the same SKU.
        string sku = dto.SKU.Trim();

        //if (string.IsNullOrEmpty(sku)) // confirm if needed bcz dto has required check already, might dupicate that
        //    throw new ArgumentException("SKU is required."); 

        if (await _repository.SkuExistsAsync(sku))
            throw new ArgumentException($"A product with this sku ({sku}) already exists.");

            var product = new Product
        {
            SKU = sku,
            Name = dto.Name.Trim(),
            Category = dto.Category.Trim(),
            UnitPrice = dto.UnitPrice,
            IsActive = true,
            StockQuantity = dto.StockQuantity,
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
            UpdatedAt = null
        };

        var createdProduct = await _repository.AddAsync(product);

        return MapToDto(createdProduct);
    }

    // PUT 
    public async Task<bool> UpdateAsync(UpdateProductRequestDto dto, int id)
    {
        var product = await _repository.GetByIdAsync(id);

        if (product == null)
            return false;

        string sku = dto.SKU.Trim();

        // Exclude the current product; otherwise every update would conflict with
        // its own existing SKU.
        if (await _repository.SkuExistsAsync(sku, id))
            throw new ArgumentException
                ($"A product with this sku ({sku}) already exists.");

        product.SKU = sku;
        product.Name = dto.Name.Trim();
        product.Category = dto.Category.Trim();
        product.UnitPrice = dto.UnitPrice;
        product.StockQuantity = dto.StockQuantity;
        product.IsActive = dto.IsActive;
        //product.CreatedAt = dto.CreatedAt,
        product.UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow);

        await _repository.UpdateAsync(id);

        return true;
    }

    public async Task<Product?> UpdateActiveStatusAsync(int id, bool isActive)
    {
        // This is a soft delete/reactivation: the row remains available for old
        // orders, but inactive products can be hidden from normal product lists.
        var product = await _retailDbContext.Products.FindAsync(id);

        // The controller converts null into a 404 response.
        if (product == null)
        {
            return null;
        }

        product.IsActive = isActive;

        await _retailDbContext.SaveChangesAsync();

        return product;
    }

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
