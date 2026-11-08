using Microsoft.EntityFrameworkCore;
using RetailOrderStockManagementSystem.Backend.Data;
using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Models;

namespace RetailOrderStockManagementSystem.Backend.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly RetailDbContext _retailDbContext;

    public ProductRepository(RetailDbContext retailDbContext)
    {
        _retailDbContext = retailDbContext;
    }

    // GET all products (active/inactive)
    public async Task<List<Product>> GetAllAsync(ProductQueryParameters parameters)
    {
        var query = _retailDbContext.Products.AsQueryable();

        // Build the query first and execute it only at ToListAsync. This lets
        // EF Core translate filtering, sorting, and paging into one SQL query.
        if (!parameters.IncludeInactive)
            query = query.Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(search));
        }

        query = parameters.SortBy?.ToLower() switch
        {
            "price" => parameters.Descending ? query.OrderByDescending(p => p.UnitPrice) :
            query.OrderBy(p => p.UnitPrice),
            "name" => parameters.Descending ? query.OrderByDescending(p => p.Name) :
            query.OrderBy(p => p.Name),
            _ => query.OrderBy(p => p.ProductId)
        };

        // Skip/Take returns one page instead of loading every product into memory.
        query = query.Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize);

        return await query.ToListAsync();
    }

    // GET product ById, returns null if product is not found
    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _retailDbContext.Products.FindAsync(id);
    }

    // POST new product
    public async Task<Product> AddAsync(Product product)
    {
        _retailDbContext.Products.Add(product);

        await _retailDbContext.SaveChangesAsync();

        return product;
    }

    // PUT product using ProductId
    public async Task<bool> UpdateAsync(int id)
    {
        // Retrieve existing product from the database
        var product = await _retailDbContext.Products.FindAsync(id);

        // Return null if Product doesn't exist
        if (product == null)
        {
            return false;
        }

        // Update status instead of removing the record
        product.UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
        _retailDbContext.Products.Update(product);

        // Persist/save changes to the database
        await _retailDbContext.SaveChangesAsync();
        return true;
    }

    // Delete product changes product status to inactive instead of deleting the product
    public async Task<bool> DeleteAsync(int id)
    {
        // Retrieve existing product from the database
        var product = await _retailDbContext.Products.FindAsync(id);
        
        // Return null if Product doesn't exist
        if (product == null)
        {
            return false;
        }

        // Update status instead of removing the record
        product.IsActive = false;
        product.UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow);

        // Persist/save changes to the database
        await _retailDbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SkuExistsAsync(string sku, int? excludedProductId = null)
    {
        var query = _retailDbContext.Products.Where(product => product.SKU == sku);

        // During an update, ignore the row being edited when checking uniqueness.
        if (excludedProductId.HasValue)
        {
            int productId = excludedProductId.Value;
            query = query.Where(product => product.ProductId != productId);
        }

        return await query.AnyAsync();
    }

}
