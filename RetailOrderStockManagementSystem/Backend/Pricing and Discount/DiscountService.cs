using RetailOrderStockManagementSystem.Backend.Data;
using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace RetailOrderStockManagementSystem.Backend.Pricing_and_Discount;

public class DiscountService
{
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
        decimal highestPercentage = 0m;
        string discountName = "No discount";
        var rules = await _retailDbContext.DiscountRules
            .AsNoTracking()
            .Where(rule => rule.IsActive)
            .ToListAsync();

        // Rules are identified by their business names because each rule type has
        // different eligibility conditions.
        var largeOrderRule = rules.FirstOrDefault(rule =>
            rule.Name == "Large Order Discount");
        var firstTimeCustomerRule = rules.FirstOrDefault(rule =>
            rule.Name == "First-Time Customer Discount");
        var loyaltyRule = rules.FirstOrDefault(rule =>
            rule.Name == "Loyal Customer Discount");

        // Only the largest applicable discount is used; discounts are not stacked.
        if (largeOrderRule is not null &&
            subtotal >= largeOrderRule.MinimumOrderAmount &&
            largeOrderRule.Percent / 100m > highestPercentage)
        {
            highestPercentage = largeOrderRule.Percent / 100m;
            discountName = largeOrderRule.Name;
        }

        if (customer.FirstTimeCustomer && firstTimeCustomerRule is not null &&
            firstTimeCustomerRule.Percent / 100m > highestPercentage)
        {
            highestPercentage = firstTimeCustomerRule.Percent / 100m;
            discountName = firstTimeCustomerRule.Name;
        }

        if (customer.LoyalCustomer && loyaltyRule is not null &&
            loyaltyRule.Percent / 100m > highestPercentage)
        {
            highestPercentage = loyaltyRule.Percent / 100m;
            discountName = loyaltyRule.Name;
        }

        return new DiscountResult
        {
            DiscountName = discountName,
            DiscountPercent = highestPercentage,
            DiscountAmount = subtotal * highestPercentage
        };
    }
}
