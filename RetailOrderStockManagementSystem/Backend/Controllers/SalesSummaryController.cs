using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailOrderStockManagementSystem.Backend.DTOs;

namespace RetailOrderStockManagementSystem.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesSummaryController : ControllerBase
    {
        private readonly IMapper _mapper;
        public SalesSummaryController(IMapper mapper)
        {
            _mapper = mapper;
        }
        [Authorize(Roles ="Manager")]
        [HttpGet]
        public async Task<IActionResult> GetSalesSummaryReport()
        {
            
            return Ok();
        }
    }
}
