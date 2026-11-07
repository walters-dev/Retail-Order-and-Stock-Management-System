using Microsoft.AspNetCore.Mvc;
using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Enums;


//using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Models;
using RetailOrderStockManagementSystem.Backend.Services;

namespace RetailOrderStockManagementSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    public async Task<IActionResult> GetAll([FromQuery] OrderStatus.Status? status = null)
    {
        return Ok(await _orderService.GetAllAsync(status));
    }

    // GET/api/orders/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _orderService.GetByIdAsync(id);

        if (order == null)
            return NotFound(new { message = $"Order {id} was not found." } );

        return Ok(order);
    }

    // PUT/api/orders/{id}/confirm
    [HttpPut("{id:int}/confirm")]
    public async Task<IActionResult> Confirm(int id,
        CancellationToken cancellationToken)
    {
        return await ChangeStatus(id, () => 
        _orderService.ConfirmAsync(id, cancellationToken), 
        "Order confirmed; stock was reduced.", cancellationToken);

    }

    // PUT/api/orders/{id}/complete
    [HttpPut("{id:int}/complete")]
    public async Task<IActionResult> Complete(int id,
        CancellationToken cancellationToken)
    {
        return await ChangeStatus(id, () => 
        _orderService.CompleteAsync(id, cancellationToken), 
        "Order completed.", cancellationToken);

    }

    // PUT/api/orders/{id}/cancel
    [HttpPut("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id,
        CancellationToken cancellationToken)
    {
        return await ChangeStatus(id, () =>
        _orderService.CancelAsync(id, cancellationToken), 
        "Pending order cancelled.", cancellationToken);

    }

    private async Task<IActionResult> ChangeStatus(int id, Func<Task<bool>> change, string message, CancellationToken cancellationToken)
    {
        if (!await change())
        {
            if (await _orderService.GetByIdAsync(id) == null) return NotFound(new { message = $"Order {id} was not found." });
            return Conflict(new { message = "The order status transition is not allowed, or stock is insufficient to confirm the order." });
        }
        return Ok(new { message, order = await _orderService.GetByIdAsync(id) });
    }

}
