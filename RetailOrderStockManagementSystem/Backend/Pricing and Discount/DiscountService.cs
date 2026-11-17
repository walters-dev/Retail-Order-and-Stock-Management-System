using Microsoft.EntityFrameworkCore;
using RetailOrderStockManagementSystem.Backend.Data;
using RetailOrderStockManagementSystem.Backend.Models;

// Evaluates active discount rules from the database to find the single best discount for an order.
public sealed class DiscountService
{
    private readonly RetailDbContext _dbContext;

    public DiscountService(RetailDbContext dbContext)
    {
        _dbContext = dbContext;
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

        // Load only active rules as read-only (no change tracking) for better query performance.
        var activeRules = await _dbContext.DiscountRules
            .AsNoTracking()
            .Where(rule => rule.IsActive)
            .ToListAsync(cancellationToken);

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
