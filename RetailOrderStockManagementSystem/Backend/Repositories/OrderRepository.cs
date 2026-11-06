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

    // GET all orders
    public async Task<List<Order>> GetAllAsync()
    {
        return await _retailDbContext.Orders.ToListAsync();
    }

    // GET orders ById, returns null if order is not found
    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _retailDbContext.Orders.FindAsync(id);
    }

    // POST new order, create a pending order
    public async Task<Order> AddAsync(Order order)
    {
        _retailDbContext.Orders.Add(order);
        await _retailDbContext.SaveChangesAsync();
        return order;
    }

    // PUT order, update stock
    public async Task UpdateAsync(Order order)
    {
        _retailDbContext.Orders.Update(order);
        await _retailDbContext.SaveChangesAsync();
    }
}
