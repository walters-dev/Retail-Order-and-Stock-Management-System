using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Enums;
using RetailOrderStockManagementSystem.Backend.Models;
using RetailOrderStockManagementSystem.Backend.Repositories;

namespace RetailOrderStockManagementSystem.Backend.Services
{
    public class ReportService : IReportService
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public ReportService(IMapper mapper, ILogger<ReportService> logger, IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }
        public async Task<(int count, int totalCompleted, decimal totalRevenue, List<OrderResponseAdminDto> orders)> GetSalesSummaryAsync()
        {
            //oder count
            var orders = await _orderRepository.GetAllAsync();
            int orderCount = orders.Count();
            //total completed sales
            List<OrderResponseAdminDto> ordersCompleted = _mapper.Map<List<Order>, List < OrderResponseAdminDto >> (await _orderRepository.GetAllAsync(OrderStatus.Status.Completed));
            int totalCompletedSales = ordersCompleted.Count;
            decimal totalEarnings = 0;
            foreach (OrderResponseAdminDto order in ordersCompleted)
            {
                totalEarnings += order.FinalTotal;
            }
            return (orderCount, totalCompletedSales, totalEarnings, ordersCompleted);
        }
        public async Task<List<ProductResponseAdminDto>> GetLowStockAsync(ProductQueryParameters productQuery)
        {
            List<Product> products = await _productRepository.GetAllAsync(productQuery);
            List<ProductResponseAdminDto> lowStockProducts = new List<ProductResponseAdminDto>();
            foreach (Product product in products)
            {
                if (product.StockQuantity <=50)
                {
                    lowStockProducts.Add(_mapper.Map<ProductResponseAdminDto >(product));
                }
            }
            
            return lowStockProducts;
        }
    }
}
