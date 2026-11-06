using RetailOrderStockManagementSystem.Backend.Enums;
using RetailOrderStockManagementSystem.Backend.Models;

namespace RetailOrderStockManagementSystem.Backend.Repositories;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync(OrderStatus.Status? status = null);
    Task<Order?> GetByIdAsync(int id);
    Task<Order> AddAsync(Order order);
    //Task<bool> ConfirmAsync(int id);
    //Task<bool> SetStatusAsync(int id, OrderStatus.Status from, OrderStatus.Status to);


    //Task UpdateAsync(Order order);
    //Task DeleteAsync(Order order); might be needed if business rules change
}
