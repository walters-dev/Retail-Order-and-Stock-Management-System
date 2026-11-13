using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Enums;
using RetailOrderStockManagementSystem.Backend.Models;

namespace RetailOrderStockManagementSystem.Backend.Services;

public interface IOrderService
{
    Task<List<OrderResponseAdminDto>> GetAllAsync(OrderStatus.Status? status = null);
    Task<OrderDetailDto?> GetByIdAsync(Guid id);
    Task<OrderDetailDto> CreateAsync(CreateOrderRequestDto dto);
    Task<OrderQuoteDto> QuoteAsync(CreateOrderRequestDto dto);
    Task<bool> ConfirmAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> CompleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> CancelAsync(Guid id, CancellationToken cancellationToken = default);
}
