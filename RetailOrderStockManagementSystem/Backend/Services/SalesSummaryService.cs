using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using RetailOrderStockManagementSystem.Backend.Models;
using RetailOrderStockManagementSystem.Backend.Repositories;

namespace RetailOrderStockManagementSystem.Backend.Services
{
    public class SalesSummaryService
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IOrderRepository _orderRepository;
        public SalesSummaryService(IMapper mapper, ILogger logger, IOrderRepository orderRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _orderRepository = orderRepository;
        }
        public async Task<> GetSalesSummary()
        {
            //oder count
            int orderCount = await _orderRepository.GetOrders().count;
            //total completed sales
            List<Order> orders = await _orderRepository.GetCompletedOrders();
            int totalCompletedSales = orders.Count;
            decimal totalEarnings = 0;
            foreach (Order order in orders)
            {
                totalEarnings += order.FinalTotal;
            }
            
        }
    }
}
