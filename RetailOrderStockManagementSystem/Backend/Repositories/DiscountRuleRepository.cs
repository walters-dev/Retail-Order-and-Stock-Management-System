using Microsoft.EntityFrameworkCore;
using RetailOrderStockManagementSystem.Backend.Models;

namespace RetailOrderStockManagementSystem.Backend.Data.Repositories;

public sealed class DiscountRuleRepository : IDiscountRuleRepository
{
    private readonly RetailDbContext _dbContext;

    public DiscountRuleRepository(RetailDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<DiscountRule>> GetActiveRulesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.DiscountRules
            .AsNoTracking()
            .Where(rule => rule.IsActive)
            .ToListAsync(cancellationToken);
    }
}
