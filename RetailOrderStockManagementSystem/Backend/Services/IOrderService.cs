using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Enums;
using RetailOrderStockManagementSystem.Backend.Models;

namespace RetailOrderStockManagementSystem.Backend.Services;

public interface IOrderService
{
    Task<List<OrderResponseAdminDto>> GetAllAsync(OrderStatus.Status? status = null);
    Task<OrderDetailDto?> GetByIdAsync(int id);
    Task<OrderDetailDto> CreateAsync(CreateOrderRequestDto dto);
    Task<OrderDetailDto> QuoteAsync(CreateOrderRequestDto dto);
    Task<bool> ConfirmAsync(int id);
    Task<bool> CompleteAsync(int id);
    Task<bool> CancelAsync(int id);
}
