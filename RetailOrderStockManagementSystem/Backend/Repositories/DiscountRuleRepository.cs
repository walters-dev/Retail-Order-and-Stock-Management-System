using Microsoft.EntityFrameworkCore;
using RetailOrderStockManagementSystem.Backend.Data;
using RetailOrderStockManagementSystem.Backend.Models;

namespace RetailOrderStockManagementSystem.Backend.Repositories;

/// <summary>
/// Loads active discount rules used by the discount-selection policy.
/// </summary>
public sealed class DiscountRuleRepository : IDiscountRuleRepository
{
    private readonly RetailDbContext _context;

    public DiscountRuleRepository(RetailDbContext context)
    {
        _context = context;
    }

    public Task<List<DiscountRule>> GetActiveRulesAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.DiscountRules
            .AsNoTracking()
            .Where(rule => rule.IsActive)
            .ToListAsync(cancellationToken);
    }
}
