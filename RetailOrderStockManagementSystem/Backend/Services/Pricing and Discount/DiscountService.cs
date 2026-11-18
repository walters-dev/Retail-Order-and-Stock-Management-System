using RetailOrderStockManagementSystem.Backend.Data.Repositories;
using RetailOrderStockManagementSystem.Backend.Models;

namespace RetailOrderStockManagementSystem.Backend.Services;

// Evaluates active discount rules to find the single best discount for an order.
public sealed class DiscountService
{
    private readonly IDiscountRuleRepository _discountRuleRepository;

    public DiscountService(IDiscountRuleRepository discountRuleRepository)
    {
        _discountRuleRepository = discountRuleRepository;
    }

    // Returns the highest eligible active discount rule, or null when no rule applies.
    // Discounts do not stack; ties are broken by the lowest DiscountRuleId.
    public async Task<DiscountRule?> SelectDiscountRuleAsync(
        Customer customer,
        decimal subtotal,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(customer);

        if (subtotal < 0m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(subtotal), subtotal, "Subtotal cannot be negative.");
        }

        var activeRules = await _discountRuleRepository.GetActiveRulesAsync(cancellationToken);

        // Filter to rules the customer qualifies for, then pick the one with the highest percentage.
        return activeRules
            .Where(rule => IsEligible(rule, customer, subtotal))
            .OrderByDescending(rule => rule.Percent)
            .ThenBy(rule => rule.DiscountRuleId)
            .FirstOrDefault();
    }

    // Checks whether the customer and order meet the condition for a specific discount rule.
    private static bool IsEligible(
        DiscountRule rule,
        Customer customer,
        decimal subtotal)
    {
        return rule.Name.Trim() switch
        {
            "Large Order Discount" => subtotal >= rule.MinimumOrderAmount,
            "First Time Customer Discount" => customer.FirstTimeCustomer,
            "Loyalty Discount" => customer.LoyaltyCustomer,
            _ => false
        };
    }
}
