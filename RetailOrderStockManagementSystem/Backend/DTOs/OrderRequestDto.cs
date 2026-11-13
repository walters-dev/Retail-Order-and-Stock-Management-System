namespace RetailOrderStockManagementSystem.Backend.DTOs;

public sealed class OrderLineRequestDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public sealed class CreateOrderRequestDto
{
    public Guid CustomerId { get; set; }
    public List<OrderLineRequestDto> Items { get; set; } = new();
}

public sealed class OrderQuoteDto
{
    public Guid CustomerId { get; set; }
    public List<OrderLineQuoteDto> Items { get; set; } = new();
    public int? DiscountRuleId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalTotal { get; set; }
}

public sealed class OrderLineQuoteDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}

public sealed class OrderDetailDto : OrderResponseAdminDto
{
    public CustomerOrderDto? Customer { get; set; }
    public List<OrderLineDetailDto> Items { get; set; } = new();
}

public sealed class CustomerOrderDto
{
    public Guid CustomerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
}

public sealed class OrderLineDetailDto
{
    public int OrderLineItemId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}
