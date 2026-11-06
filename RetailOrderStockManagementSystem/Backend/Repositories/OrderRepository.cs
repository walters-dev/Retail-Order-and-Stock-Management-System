using Microsoft.EntityFrameworkCore;
using RetailOrderStockManagementSystem.Backend.Data;
using RetailOrderStockManagementSystem.Backend.Enums;
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
    public async Task<List<Order>> GetAllAsync(OrderStatus.Status? status = null)
    {
        var query = _retailDbContext.Orders.AsNoTracking().AsQueryable();
        if (status.HasValue) query = query.Where(order => order.Status == status.Value);
        return await query.OrderByDescending(order => order.OrderDate)
            .ThenByDescending(order => order.OrderId).ToListAsync();
    }

    // GET orders ById, returns null if order is not found
    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _retailDbContext.Orders.AsNoTracking()
            .Include(order => order.CustomerId)
            .Include(order => order.OrderLineItems).ThenInclude(line => line.ProductId)
            .SingleOrDefaultAsync(order => order.OrderId == id);
    }

    // POST new order, create a pending order
    public async Task<Order> AddAsync(Order order)
    {
        _retailDbContext.Orders.Add(order);
        await _retailDbContext.SaveChangesAsync();
        return order;
    }

    //// PUT order, update stock
    //public async Task UpdateAsync(Order order)
    //{
    //    _retailDbContext.Orders.Update(order);
    //    await _retailDbContext.SaveChangesAsync();
    //}

    //public async Task<bool> ConfirmAsync(int id)
    //{

    //}

    //public async Task<bool> SetStatusAsync(int id, OrderStatus.Status from, OrderStatus.Status to)
    //{

    //}







}
