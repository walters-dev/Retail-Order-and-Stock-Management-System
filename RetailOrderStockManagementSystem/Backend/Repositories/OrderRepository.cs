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
        var query = _retailDbContext.Orders.AsNoTracking()
            .Include(order => order.DiscountRule)
            .AsQueryable();
        if (status.HasValue) query = query.Where(order => order.Status == status.Value);
        return await query.OrderByDescending(order => order.OrderDate)
            .ThenByDescending(order => order.OrderId).ToListAsync();
    }

    // GET orders ById, returns null if order is not found
    public async Task<Order?> GetByIdAsync(int id)
    {
        // Include related objects because the detail response needs customer and
        // product information; CustomerId/ProductId alone are just foreign keys.
        return await _retailDbContext.Orders.AsNoTracking()
            .Include(order => order.Customer)
            .Include(order => order.DiscountRule)
            .Include(order => order.OrderLineItems).ThenInclude(line => line.Product)
            .SingleOrDefaultAsync(order => order.OrderId == id);
    }

    // POST new order, create a pending order
    public async Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        _retailDbContext.Orders.Add(order);
        await _retailDbContext.SaveChangesAsync(cancellationToken);
        return order;
    }

    public async Task<bool> ConfirmAsync(int id, CancellationToken cancellationToken = default)
    {
        // Confirmation changes both stock and order state, so both operations must
        // succeed together or the transaction is rolled back.
        await using var transaction = 
            await _retailDbContext.Database.BeginTransactionAsync(cancellationToken);

        var order = await _retailDbContext.Orders.AsNoTracking().Include(o => o.OrderLineItems)
            .SingleOrDefaultAsync(o => o.OrderId == id, cancellationToken);

        // Confirmation is the final integrity check before stock is changed.
        // Reject malformed orders even if they did not come through the API.
        if (order == null || order.Status != OrderStatus.Status.Pending ||
            order.OrderLineItems.Count == 0 ||
            order.OrderLineItems.Any(item => item.Quantity <= 0))
            return false;

        foreach (var line in order.OrderLineItems.GroupBy(item => item.ProductId)
                   .Select(group => new 
                   { ProductId = group.Key, Quantity = group.Sum(item => item.Quantity) }))
        {
            // Grouping makes stock reduction safe when an order contains duplicate
            // lines for the same product.
            var changed = await _retailDbContext.Products
                .Where(product => product.ProductId == line.ProductId && product.StockQuantity >= line.Quantity)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(product => product.StockQuantity, product => product.StockQuantity - line.Quantity)
                    .SetProperty(product => product.UpdatedAt, DateOnly.FromDateTime(DateTime.UtcNow)), cancellationToken);

            // Affected-row count of zero means the product was missing or stock
            // became insufficient after the order was created.
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

        // A successful confirmation consumes the customer's first-time status.
        // Keep this inside the same transaction as stock and order confirmation.
        await _retailDbContext.Customers
            .Where(customer => customer.CustomerId == order.CustomerId &&
                               customer.FirstTimeCustomer)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(customer => customer.FirstTimeCustomer, false)
                .SetProperty(customer => customer.UpdatedAt,
                    DateOnly.FromDateTime(DateTime.UtcNow)), cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        return true;
    }

    public async Task<bool> SetStatusAsync(int id, OrderStatus.Status from, 
        OrderStatus.Status to, CancellationToken cancellationToken = default)
    {
        // Including the current status makes the transition conditional and
        // prevents an order from being moved through an invalid state.
        return await _retailDbContext.Orders
            .Where(order => order.OrderId == id && order.Status == from)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(order => order.Status, to)
                .SetProperty(order => order.UpdatedAt, 
                DateOnly.FromDateTime(DateTime.UtcNow)), cancellationToken) == 1;
    }
}
