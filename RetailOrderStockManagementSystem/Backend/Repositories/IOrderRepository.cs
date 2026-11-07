using RetailOrderStockManagementSystem.Backend.Enums;
using RetailOrderStockManagementSystem.Backend.Models;

namespace RetailOrderStockManagementSystem.Backend.Repositories;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync(OrderStatus.Status? status = null);
    Task<Order?> GetByIdAsync(int id);
    Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default);
    Task<bool> ConfirmAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> SetStatusAsync(int id, OrderStatus.Status from, OrderStatus.Status to, 
        CancellationToken cancellationToken = default);
}
