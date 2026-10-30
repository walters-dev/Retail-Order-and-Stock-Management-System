using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Models;

namespace RetailOrderStockManagementSystem.Backend.Services;

public interface IOrderService
{
    Task<List<OrderResponseAdminDto>> GetAllAsync();
    Task<OrderResponseAdminDto?> GetByIdAscync(int id);
    Task<OrderResponseAdminDto> CreateAsync(Order order);
}
