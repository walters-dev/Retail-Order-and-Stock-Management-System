//using Azure.Core;
using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Enums;
using RetailOrderStockManagementSystem.Backend.Models;
using RetailOrderStockManagementSystem.Backend.Repositories;
using System.ComponentModel.DataAnnotations;

namespace RetailOrderStockManagementSystem.Backend.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orders;
    private readonly ICustomerRepository _customers;
    private readonly IProductRepository _products;
    private readonly PricingService _pricing;
    private readonly DiscountService _discounts;

    public OrderService(IOrderRepository orders, ICustomerRepository customers,
        IProductRepository products, PricingService pricing, DiscountService discounts)
    {
        _orders = orders;
        _customers = customers;
        _products = products;
        _pricing = pricing;
        _discounts = discounts;
    }

    public async Task<List<OrderResponseAdminDto>> GetAllAsync(OrderStatus.Status? status = null)
    {
        var orders = await _orders.GetAllAsync(status);
        return orders.Select(MapToDto).ToList();
    }

    public async Task<OrderDetailDto?> GetByIdAsync(int id)
    {
        var order = await _orders.GetByIdAsync(id);
        return order == null ? null : MapToDetailDto(order);
    }

    public async Task<OrderQuoteDto> QuoteAsync(CreateOrderRequestDto dto)
    {
        var (customer, items, quote) = await CalculateAsync(dto);
        return quote;
    }

    public async Task<OrderDetailDto> CreateAsync(CreateOrderRequestDto dto)
    {
        // CalculateAsync performs all validation and creates line items before the
        // order is persisted, so the saved order uses the quoted values.
        var (customer, lines, quote) = await CalculateAsync(dto);

        var order = new Order
        {
            CustomerId = customer.CustomerId,
            OrderDate = DateOnly.FromDateTime(DateTime.UtcNow),
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
            Status = OrderStatus.Status.Pending,
            Subtotal = quote.Subtotal,
            DiscountPercent = quote.DiscountPercent,
            DiscountAmount = quote.DiscountAmount,
            FinalTotal = quote.FinalTotal,
            OrderLineItems = lines
        };

        await _orders.AddAsync(order);

        // The order and its line items have already been saved. Build the response
        // from the saved entities and quote data instead of querying the database
        // again; a failed reload must not make a successful POST look unsuccessful.
        order.Customer = customer;

        var productNames = quote.Items.ToDictionary(
            item => item.ProductId,
            item => item.ProductName);

        foreach (var line in order.OrderLineItems)
        {
            if (productNames.TryGetValue(line.ProductId, out var productName))
            {
                line.Product = new Product
                {
                    ProductId = line.ProductId,
                    Name = productName
                };
            }
        }

        return MapToDetailDto(order);
    }

    public Task<bool> ConfirmAsync(int id, CancellationToken cancellationToken = default)
    {
        return _orders.ConfirmAsync(id, cancellationToken);

    }

    public Task<bool> CompleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return _orders.SetStatusAsync(id, OrderStatus.Status.Confirmed, 
            OrderStatus.Status.Completed, cancellationToken);
    }

    public Task<bool> CancelAsync(int id, CancellationToken cancellationToken = default)
    {
        return _orders.SetStatusAsync(id, OrderStatus.Status.Pending, 
            OrderStatus.Status.Cancelled, cancellationToken);
    }

    private async Task<(Customer Customer, List<OrderLineItem> Lines, OrderQuoteDto Quote)> CalculateAsync(CreateOrderRequestDto request)
    {
        if (request == null) 
            throw new ValidationException("An order request is required.");

        if (request.CustomerId == Guid.Empty) 
            throw new ValidationException("A valid customer is required.");

        if (request.Items == null || request.Items.Count == 0) 
            throw new ValidationException("An order must contain at least one line item.");

        var customer = await _customers.GetByIdAsync(request.CustomerId);

        if (customer == null || !customer.IsActive) 
            throw new KeyNotFoundException("The customer does not exist or is inactive.");

        var requested = new Dictionary<int, int>();

        foreach (var item in request.Items)
        {
            if (item.ProductId <= 0) 
                throw new ValidationException("Each line item must have a valid product ID.");

            if (item.Quantity <= 0) 
                throw new ValidationException("Order quantities must be positive whole numbers.");

            // Combining duplicate product lines prevents the same product from
            // being validated and priced separately more than once.
            requested[item.ProductId] = 
                checked(requested.GetValueOrDefault(item.ProductId) + item.Quantity);
        }

        var lines = new List<OrderLineItem>();

        var quoteLines = new List<OrderLineQuoteDto>();

        foreach (var entry in requested)
        {
            var product = await _products.GetByIdAsync(entry.Key);

            if (product == null || !product.IsActive) 
                throw new KeyNotFoundException($"Product {entry.Key} does not exist or is inactive.");

            if (product.UnitPrice <= 0m) 
                throw new ValidationException($"Product '{product.Name}' " +
                    $"must have a price greater than zero before it can be ordered.");

            if (entry.Value > product.StockQuantity)
                throw new ValidationException($"Insufficient stock for '{product.Name}'. " +
                    $"Available: {product.StockQuantity}; requested: {entry.Value}.");

            // Store the price used at purchase time; later product price changes
            // must not change the history of an existing order.
            var unitPrice = 
                Math.Round(product.UnitPrice, 2, MidpointRounding.AwayFromZero);

            var lineTotal = 
                Math.Round(_pricing.CalculateLineTotal(entry.Value, unitPrice), 2, MidpointRounding.AwayFromZero);

            lines.Add(new OrderLineItem 
            { ProductId = product.ProductId, Quantity = entry.Value, 
                UnitPrice = unitPrice, LineTotal = lineTotal });

            quoteLines.Add(new OrderLineQuoteDto 
            { ProductId = product.ProductId, ProductName = product.Name, 
                Quantity = entry.Value, UnitPrice = unitPrice, LineTotal = lineTotal });
        }

        decimal subtotal = 
            Math.Round(_pricing.CalculateSubtotal(lines), 2, MidpointRounding.AwayFromZero);

        // Discount rules are evaluated against the subtotal, after all line
        // quantities and prices have been validated.
        var discount = 
            await _discounts.CalculateDiscountAsync(customer, new Order(), subtotal);

        decimal discountAmount = 
            Math.Round(discount.DiscountAmount, 2, MidpointRounding.AwayFromZero);

        decimal finalTotal = 
            Math.Max(0m, Math.Round(_pricing.CalculateFinalTotal(subtotal, discountAmount), 2, MidpointRounding.AwayFromZero));

        return (customer, lines, new OrderQuoteDto
        {
            CustomerId = customer.CustomerId,
            Items = quoteLines,
            Subtotal = subtotal,
            DiscountPercent = discount.DiscountPercent * 100m,
            DiscountAmount = discountAmount,
            FinalTotal = finalTotal
        });
    }


    private static OrderResponseAdminDto MapToDto(Order order) => new()
    {
        OrderId = order.OrderId,
        CustomerId = order.CustomerId,
        OrderDate = order.OrderDate,
        Subtotal = order.Subtotal,
        DiscountPercent = order.DiscountPercent,
        DiscountAmount = order.DiscountAmount,
        FinalTotal = order.FinalTotal,
        Status = order.Status,
        CreatedAt = order.CreatedAt,
        UpdatedAt = order.UpdatedAt
    };

    private static OrderDetailDto MapToDetailDto(Order order) => new()
    {
        OrderId = order.OrderId,
        CustomerId = order.CustomerId,
        OrderDate = order.OrderDate,
        Subtotal = order.Subtotal,
        DiscountPercent = order.DiscountPercent,
        DiscountAmount = order.DiscountAmount,
        FinalTotal = order.FinalTotal,
        Status = order.Status,
        CreatedAt = order.CreatedAt,
        UpdatedAt = order.UpdatedAt,
        Customer = order.Customer == null ? null : new CustomerOrderDto
        {
            CustomerId = order.Customer.CustomerId,
            FullName = order.Customer.FullName,
            MobileNumber = order.Customer.MobileNumber,
            Email = order.Customer.Email
        },
        Items = order.OrderLineItems.Select(line => new OrderLineDetailDto
        {
            OrderLineItemId = line.OrderLineItemId,
            ProductId = line.ProductId,
            ProductName = line.Product?.Name ?? string.Empty,
            Quantity = line.Quantity,
            UnitPrice = line.UnitPrice,
            LineTotal = line.LineTotal
        }).ToList()
    };
}
