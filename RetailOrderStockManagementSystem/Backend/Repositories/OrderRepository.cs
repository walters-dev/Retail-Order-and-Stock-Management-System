using Microsoft.EntityFrameworkCore;
using RetailOrderStockManagementSystem.Backend.Data;
using RetailOrderStockManagementSystem.Backend.Models;

namespace RetailOrderStockManagementSystem.Backend.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly RetailDbContext _retailDbContext;

    public OrderRepository(RetailDbContext retailDbContext)
    {
        _retailDbContext = retailDbContext;
    }

    public async Task<List<Order>> GetAllAsync()
    {
        return await _retailDbContext.Orders.ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _retailDbContext.Orders.FindAsync(id);
    }

    public async Task<Order> AddAsync(Order order)
    {
        _retailDbContext.Orders.Add(order);
        await _retailDbContext.SaveChangesAsync();
        return order;
    }

    public async Task UpdateAsync(Order order)
    {
        _retailDbContext.Orders.Update(order);
        await _retailDbContext.SaveChangesAsync();
    }
}
