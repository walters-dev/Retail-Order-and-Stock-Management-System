using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Models;
using RetailOrderStockManagementSystem.Backend.Repositories;

namespace RetailOrderStockManagementSystem.Backend.Services;

public class OrderService : IOrderService
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

    public async Task<OrderResponseAdminDto?> GetByIdAsync(int id)
    {
        var order = await _repository.GetByIdAsync(id);
        return order == null ? null : MapToDto(order);
    }

    public async Task<OrderResponseAdminDto> CreateAsync(Order order)
    {
        /* Add validation below (from bussiness rules)
         * Customer active? Every order must belong to a valid customer.
         * Product active? Only active products may be added to a new quote or order.
         * Order qty > 0? Order quantities must be positive whole numbers.
         * Qty < stockQty? Requested quantity may not exceed available StockQuantity.
         * 
        */



        var newOrder = new Order
        {
            OrderDate = order.OrderDate,
            Subtotal = order.Subtotal,
            FinalTotal = order.FinalTotal,
            Status = order.Status
        };

        newOrder = await _repository.AddAsync(newOrder);
        return MapToDto(newOrder);
    }

    private static OrderResponseAdminDto MapToDto(Order o) => new()
    {
        OrderId = o.OrderId,
        CustomerId = o.CustomerId,
        OrderDate = o.OrderDate,
        Subtotal = o.Subtotal,
        //DiscountPercent = order.DiscountPercent,
        DiscountAmount = o.DiscountAmount,
        FinalTotal = o.FinalTotal,
        Status = o.Status,
        CreatedAt = o.CreatedAt,
        UpdatedAt = o.UpdatedAt
    };
}
