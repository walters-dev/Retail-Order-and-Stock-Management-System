using RetailOrderStockManagementSystem.Backend.Models;

namespace RetailOrderStockManagementSystem.Backend.Repositories;

public interface IDiscountRuleRepository
{
    Task<List<DiscountRule>> GetActiveRulesAsync(CancellationToken cancellationToken = default);
}
