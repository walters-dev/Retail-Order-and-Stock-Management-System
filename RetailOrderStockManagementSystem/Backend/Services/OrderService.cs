using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Models;
using RetailOrderStockManagementSystem.Backend.Repositories;

namespace RetailOrderStockManagementSystem.Backend.Services;

public class OrderService
{
    private readonly IOrderRepository _repository;

    public OrderService(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<OrderResponseAdminDto>> GetAllAsync()
    {
        var orders = await _repository.GetAllAsync();
        return orders.Select(MapToDto).ToList();
    }

    public async Task<OrderResponseAdminDto?> GetByIdAscync(int id)
    {
        var order = await _repository.GetByIdAsync(id);
        return order == null ? null : MapToDto(order);
    }

    public async Task<OrderResponseAdminDto> CreateAsync(Order o)
    {
        var newOrder = new Order
        {
            OrderDate = o.OrderDate,
            Subtotal = o.Subtotal,
            DiscountPercent = o.DiscountPercent,
            FinalTotal = o.FinalTotal,
            Status = o.Status
        };

        newOrder = await _repository.AddAsync(newOrder);
        return MapToDto(newOrder);
    }

    private static OrderResponseAdminDto MapToDto(Order o) => new()
    {
        OrderDate = o.OrderDate,
        Subtotal = o.Subtotal,
        DiscountPercent = o.DiscountPercent,
        DiscountAmount = o.DiscountAmount,
        FinalTotal = o.FinalTotal,
        Status = o.Status,
        CreatedAt = o.CreatedAt,
        UpdatedAt = o.UpdatedAt
    };
}
