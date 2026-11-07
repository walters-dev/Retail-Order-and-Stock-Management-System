using k8s.KubeConfigModels;
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
        this._retailDbContext = retailDbContext;
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
    public async Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        _retailDbContext.Orders.Add(order);
        await _retailDbContext.SaveChangesAsync();
        return order;
    }

    public async Task<bool> ConfirmAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var transaction = 
            await _retailDbContext.Database.BeginTransactionAsync(cancellationToken);

        var order = await _retailDbContext.Orders.AsNoTracking().Include(o => o.OrderLineItems)
            .SingleOrDefaultAsync(o => o.OrderId == id, cancellationToken);

        if (order == null || order.Status != OrderStatus.Status.Pending)
            return false;

        foreach (var line in order.OrderLineItems.GroupBy(item => item.ProductId)
                   .Select(group => new 
                   { ProductId = group.Key, Quantity = group.Sum(item => item.Quantity) }))
        {
            var changed = await _retailDbContext.Products
                .Where(product => product.ProductId == line.ProductId && product.StockQuantity >= line.Quantity)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(product => product.StockQuantity, product => product.StockQuantity - line.Quantity)
                    .SetProperty(product => product.UpdatedAt, DateOnly.FromDateTime(DateTime.UtcNow)), cancellationToken);

            if (changed != 1)
                return false;
        }

        var updated = await _retailDbContext.Orders
            .Where(o => o.OrderId == id && o.Status == OrderStatus.Status.Pending)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(o => o.Status, OrderStatus.Status.Confirmed)
                .SetProperty(o => o.UpdatedAt, DateOnly.FromDateTime(DateTime.UtcNow)), cancellationToken);

        if (updated != 1)
            return false;

        await transaction.CommitAsync(cancellationToken);
        return true;
    }

    public async Task<bool> SetStatusAsync(int id, OrderStatus.Status from, 
        OrderStatus.Status to, CancellationToken cancellationToken = default)
    {
        return await _retailDbContext.Orders
            .Where(order => order.OrderId == id && order.Status == from)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(order => order.Status, to)
                .SetProperty(order => order.UpdatedAt, 
                DateOnly.FromDateTime(DateTime.UtcNow)), cancellationToken) == 1;
    }
}
