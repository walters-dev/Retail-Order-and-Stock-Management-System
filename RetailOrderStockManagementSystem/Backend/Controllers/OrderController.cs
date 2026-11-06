using Microsoft.AspNetCore.Mvc;
using RetailOrderStockManagementSystem.Backend.DTOs;

//using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Models;
using RetailOrderStockManagementSystem.Backend.Services;

namespace RetailOrderStockManagementSystem.Backend.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // POST/api/orders/quote
    [HttpPost("quote")]
    public async Task<ActionResult<OrderQuoteDto>> Quote(CreateOrderRequestDto dto)
    {
        return Ok(await _orderService.QuoteAsync(dto));
    }


    // POST/api/orders
    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderRequestDto dto)
    {
        var order = await _orderService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = order.OrderId }, order);
    }

    // GET/api/orders
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _orderService.GetAllAsync();
        return Ok(orders);
    }

    // GET/api/orders/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _orderService.GetByIdAsync(id);

        if (order == null)
            return NotFound();

        return Ok(order);
    }

    // PUT/api/orders/{id}/confirm

    // PUT/api/orders/{id}/complete

    // PUT/api/orders/{id}/cancel

}
