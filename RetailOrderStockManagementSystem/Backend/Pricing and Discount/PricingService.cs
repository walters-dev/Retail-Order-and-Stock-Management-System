using RetailOrderStockManagementSystem.Backend.Models;
using RetailOrderStockManagementSystem.Backend.DTOs;
using System.Linq;

namespace RetailOrderStockManagementSystem.Backend.Services;

public class PricingService
{
    private readonly DiscountService _discountService;

    public PricingService(DiscountService discountService)
    {
        _discountService = discountService;
    }

    public decimal CalculateLineTotal(OrderLineItem orderLineItem, Product product)
    {
        decimal quantity = orderLineItem.Quantity;
        decimal unitPrice = product.UnitPrice;

        return quantity * unitPrice;
    }

    public decimal CalculateSubtotal(List<OrderLineItem> items)
    {
        return items.Sum(item => item.LineTotal);
    }


    public decimal GetDiscountPercentage(List<OrderLineItem> items)
    {
        decimal subtotal = CalculateSubtotal(items);
        DiscountResult discount = _discountService.CalculateDiscount(subtotal);
    }








}
