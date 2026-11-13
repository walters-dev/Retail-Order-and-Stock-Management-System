using RetailOrderStockManagementSystem.Backend.Models;
using RetailOrderStockManagementSystem.Backend.Repositories;

namespace RetailOrderStockManagementSystem.Backend.Pricing_and_Discount;

/// Selects the single discount rule applicable to an order.
public sealed class DiscountService
{
    private const int LargeOrderDiscountRuleId = 1;
    private const int FirstTimeCustomerDiscountRuleId = 2;
    private const int LoyaltyDiscountRuleId = 3;

    private readonly IDiscountRuleRepository _discountRuleRepository;

    public DiscountService(IDiscountRuleRepository discountRuleRepository)
    {
        _discountRuleRepository = discountRuleRepository;
    }

    // Returns the highest eligible active discount rule, or null when no rule applies.
    // Discounts do not stack.
    public async Task<DiscountRule?> SelectDiscountRuleAsync(
        Customer customer,
        decimal subtotal,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(customer);

        if (subtotal < 0m)
            throw new ArgumentOutOfRangeException(
                nameof(subtotal), subtotal, "Subtotal cannot be negative.");

        var activeRules = await _discountRuleRepository
            .GetActiveRulesAsync(cancellationToken);

        return activeRules
            .Where(rule => IsEligible(rule, customer, subtotal))
            .OrderByDescending(rule => rule.Percent)
            .ThenBy(rule => rule.DiscountRuleId)
            .FirstOrDefault();
    }

    private static bool IsEligible(
        DiscountRule rule,
        Customer customer,
        decimal subtotal)
    {
        return rule.DiscountRuleId switch
        {
            LargeOrderDiscountRuleId =>
                subtotal >= rule.MinimumOrderAmount,
            FirstTimeCustomerDiscountRuleId =>
                customer.FirstTimeCustomer,
            LoyaltyDiscountRuleId =>
                customer.LoyalCustomer,
            _ => false
        };
    }
}
