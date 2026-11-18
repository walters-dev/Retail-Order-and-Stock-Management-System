using RetailOrderStockManagementSystem.Backend.Models;

namespace RetailOrderStockManagementSystem.Backend.Data.Repositories;

public interface IDiscountRuleRepository
{
    Task<IReadOnlyList<DiscountRule>> GetActiveRulesAsync(CancellationToken cancellationToken = default);
}
