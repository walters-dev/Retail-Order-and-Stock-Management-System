using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Services;

namespace RetailOrderStockManagementSystem.Backend.Controllers
{
    [ApiController]
    [Route("api/reports")]
    [Authorize(Roles = "Manager")]
    public class ReportController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IReportService _service;
        public ReportController(IMapper mapper, IReportService service)
        {
            _mapper = mapper;
            _service = service;
        }
        //[Authorize(Roles ="Manager")]
        [HttpGet("sales-summary")]
        public async Task<IActionResult> GetSalesSummaryReport()
        {
            var salesReport = await _service.GetSalesSummaryAsync();
            return Ok(new { salesReport.count, salesReport.totalCompleted, salesReport.totalRevenue, salesReport.orders });
        }
        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStockReport([FromQuery] ProductQueryParameters productQuery)
        {
            return Ok( await _service.GetLowStockAsync(productQuery));
        }
    }
}
