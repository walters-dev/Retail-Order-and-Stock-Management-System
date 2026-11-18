using RetailOrderStockManagementSystem.Backend.Models;

namespace RetailOrderStockManagementSystem.Backend.Pricing_and_Discount;

public interface IPricingService
{
    decimal CalculateLineTotal(int quantity, decimal unitPrice);

    Task<OrderPricingResult> CalculateOrderTotalsAsync(
        Customer customer,
        IEnumerable<(int Quantity, decimal UnitPrice)> lineItems);
}
