using RetailOrderStockManagementSystem.Backend.Models;
using System.ComponentModel.DataAnnotations;

namespace RetailOrderStockManagementSystem.Backend.Pricing_and_Discount;

public sealed class PricingService : IPricingService
{
    private readonly DiscountService _discountService;

    public PricingService(DiscountService discountService)
    {
        _discountService = discountService;
    }

    public decimal CalculateLineTotal(int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new ValidationException("Order quantities must be positive whole numbers.");

        if (unitPrice <= 0m)
            throw new ValidationException("Unit price must be greater than zero.");

        return RoundCurrency(quantity * unitPrice);
    }

    public async Task<OrderPricingResult> CalculateOrderTotalsAsync(
        Customer customer,
        IEnumerable<(int Quantity, decimal UnitPrice)> lineItems)
    {
        ArgumentNullException.ThrowIfNull(customer);
        ArgumentNullException.ThrowIfNull(lineItems);

        var items = lineItems.ToList();
        if (items.Count == 0)
            throw new ValidationException("An order must contain at least one line item.");

        var subtotal = RoundCurrency(items.Sum(item =>
            CalculateLineTotal(item.Quantity, item.UnitPrice)));

        var discountRule = await _discountService.SelectDiscountRuleAsync(
            customer, subtotal);

        var discountPercent = discountRule?.Percent ?? 0m;
        var discountAmount = RoundCurrency(
            subtotal * (discountPercent / 100m));
        var finalTotal = Math.Max(0m,
            RoundCurrency(subtotal - discountAmount));

        return new OrderPricingResult
        {
            Subtotal = subtotal,
            DiscountPercent = discountPercent,
            DiscountAmount = discountAmount,
            FinalTotal = finalTotal,
            DiscountRuleId = discountRule?.DiscountRuleId
        };
    }

    private static decimal RoundCurrency(decimal value)
    {
        return Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}
