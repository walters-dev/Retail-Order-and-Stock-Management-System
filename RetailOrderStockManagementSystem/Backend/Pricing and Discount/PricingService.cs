using RetailOrderStockManagementSystem.Backend.Models;
using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Pricing_and_Discount;
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
        // Line totals are calculated from the captured unit price, not the current
        // product record, so an order remains financially reproducible.
        return quantity * unitPrice;
    }

    public decimal CalculateSubtotal(List<OrderLineItem> items)
    {
        // The subtotal is the total before discounts or other order-level changes.
        return items.Sum(item => item.LineTotal);
    }

    public async Task<decimal> GetDiscountPercentageAsync(Customer customer, Order order,
        decimal subtotal)
    {
        var discount = await _discountService.CalculateDiscountAsync(customer, order, subtotal);
        return discount.DiscountPercent;
    }

    public async Task<decimal> GetDiscountAmountAsync(Customer customer, Order order,
        decimal subtotal)
    {
        var discount = await _discountService.CalculateDiscountAsync(customer, order, subtotal);
        return discount.DiscountAmount;
    }

    public decimal CalculateFinalTotal(decimal subtotal, decimal discountAmount)
    {
        // Keep this method focused on arithmetic; callers decide how to round and
        // whether business rules such as a minimum total should apply.
        return subtotal - discountAmount;
    }




    public async Task<PricingResult> CalculateOrderTotalsAsync(
    Customer customer,
    List<OrderLineItem> items)
    {
        // Round at the order boundaries so values stored and returned to clients
        // consistently use currency precision.
        decimal subtotal = Math.Round(
            CalculateSubtotal(items),
            2,
            MidpointRounding.AwayFromZero);

        // DiscountService selects the best applicable rule; pricing only applies
        // the resulting percentage and amount.
        DiscountResult discount =
            await _discountService.CalculateDiscountAsync(customer, new Order(), subtotal);

        decimal discountAmount = Math.Round(
            discount.DiscountAmount,
            2,
            MidpointRounding.AwayFromZero);

        decimal finalTotal = Math.Max(
            0m,
            Math.Round(
                CalculateFinalTotal(subtotal, discountAmount),
                2,
                MidpointRounding.AwayFromZero));

        var results = new PricingResult(
            subtotal,
            discount.DiscountPercent * 100m,
            discountAmount,
            finalTotal);

        return results;
    }

}
