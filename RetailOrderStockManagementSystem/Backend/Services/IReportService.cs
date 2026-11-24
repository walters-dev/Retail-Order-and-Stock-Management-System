using RetailOrderStockManagementSystem.Backend.DTOs;

namespace RetailOrderStockManagementSystem.Backend.Services
{
    public interface IReportService
    {
        public Task<(int count, int totalCompleted, decimal totalRevenue, List<OrderResponseAdminDto> orders)> GetSalesSummaryAsync();
        public Task<List<ProductResponseAdminDto>> GetLowStockAsync(ProductQueryParameters productQuery);
    }
}
