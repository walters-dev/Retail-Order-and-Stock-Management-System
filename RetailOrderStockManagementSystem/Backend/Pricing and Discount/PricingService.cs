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

    public decimal CalculateLineTotal(int quantity, decimal unitPrice)
    {
        return quantity * unitPrice;
    }

    public decimal CalculateSubtotal(List<OrderLineItem> items)
    {
        return items.Sum(item => item.LineTotal);
    }


    public decimal GetDiscountPercentage(Customer customer, Order order, 
        decimal subtotal)
    {
        return _discountService.
            CalculateDiscount(customer, order , subtotal).DiscountPercentage;
    }

    public decimal GetDiscountAmount(Customer customer, Order order,
        decimal subtotal)
    {
        return _discountService.
            CalculateDiscount(customer, order, subtotal).DiscountAmount;
    }

    public decimal CalculateFinalTotal(decimal subtotal, decimal discountAmount)
    {
        return subtotal - discountAmount;
    }
}
