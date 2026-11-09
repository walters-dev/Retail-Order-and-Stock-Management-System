using RetailOrderStockManagementSystem.Backend.Data;
using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace RetailOrderStockManagementSystem.Backend.Pricing_and_Discount;

public class DiscountService
{
    // This ID is seeded in Supabase and identifies the business rule rather than
    // relying on a display name that could be changed by an administrator.
    private const int LargeOrderDiscountRuleId = 1;
    private const int FirstTimeCustomerDiscountRuleId = 2;
    private const int LoyaltyDiscountRuleId = 3;

    private readonly RetailDbContext _retailDbContext;

    public DiscountService(RetailDbContext retailDbContext)
    {
        _retailDbContext = retailDbContext;
    }

    public async Task<DiscountResult> CalculateDiscountAsync(Customer customer, Order order,
        decimal subtotal)
    {
        // Percentages are converted from stored whole percentages (for example,
        // 10 means 10%) to decimal rates (0.10) before calculating the amount.
        var rules = await _retailDbContext.DiscountRules
            .AsNoTracking()
            .Where(rule => rule.IsActive &&
                (rule.DiscountRuleId == LargeOrderDiscountRuleId ||
                 rule.DiscountRuleId == FirstTimeCustomerDiscountRuleId ||
                 rule.DiscountRuleId == LoyaltyDiscountRuleId))
            .ToListAsync();

        var largeOrderRule = rules.SingleOrDefault(rule =>
            rule.DiscountRuleId == LargeOrderDiscountRuleId);
        var firstTimeCustomerRule = rules.SingleOrDefault(rule =>
            rule.DiscountRuleId == FirstTimeCustomerDiscountRuleId);
        var loyaltyRule = rules.SingleOrDefault(rule =>
            rule.DiscountRuleId == LoyaltyDiscountRuleId);

        decimal highestPercentage = 0m;
        int? discountRuleId = null;
        string discountName = "No discount";

        // The rule identity is fixed by DiscountRuleId instead of its mutable name.
        if (largeOrderRule is not null &&
            subtotal >= largeOrderRule.MinimumOrderAmount)
        {
            highestPercentage = largeOrderRule.Percent / 100m;
            discountRuleId = largeOrderRule.DiscountRuleId;
            discountName = largeOrderRule.Name;
        }

        if (customer.FirstTimeCustomer && firstTimeCustomerRule is not null &&
            firstTimeCustomerRule.Percent / 100m > highestPercentage)
        {
            highestPercentage = firstTimeCustomerRule.Percent / 100m;
            discountRuleId = firstTimeCustomerRule.DiscountRuleId;
            discountName = firstTimeCustomerRule.Name;
        }

        if (customer.LoyalCustomer && loyaltyRule is not null &&
            loyaltyRule.Percent / 100m > highestPercentage)
        {
            highestPercentage = loyaltyRule.Percent / 100m;
            discountRuleId = loyaltyRule.DiscountRuleId;
            discountName = loyaltyRule.Name;
        }

        return new DiscountResult
        {
            DiscountRuleId = discountRuleId,
            DiscountName = discountName,
            DiscountPercent = highestPercentage,
            DiscountAmount = subtotal * highestPercentage
        };
    }
}
