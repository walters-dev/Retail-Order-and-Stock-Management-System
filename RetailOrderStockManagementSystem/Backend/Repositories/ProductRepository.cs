using Microsoft.EntityFrameworkCore;
using RetailOrderStockManagementSystem.Backend.Data;
using RetailOrderStockManagementSystem.Backend.Models;

namespace RetailOrderStockManagementSystem.Backend.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly RetailDbContext _retailDbContext;

    public ProductRepository(RetailDbContext retailDbContext)
    {
        _retailDbContext = retailDbContext;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _retailDbContext.Products.ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _retailDbContext.Products.FindAsync(id);
    }

    public async Task<Product> AddAsync(Product product)
    {
        _retailDbContext.Products.Add(product);
        await _retailDbContext.SaveChangesAsync();
        return product;
    }

    public async Task UpdateAsync(Product product)
    {
        _retailDbContext.Products.Update(product);
        await _retailDbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Product product)
    {
        _retailDbContext.Products.Remove(product);
        await _retailDbContext.SaveChangesAsync();
    }
}
