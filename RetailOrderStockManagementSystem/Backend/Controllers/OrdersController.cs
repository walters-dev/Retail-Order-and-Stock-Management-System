using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Enums;
using RetailOrderStockManagementSystem.Backend.Services;
using System.ComponentModel.DataAnnotations;

namespace RetailOrderStockManagementSystem.Backend.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize(Roles = "Admin, Staff")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // POST/api/orders/quote
    [Authorize(Roles = "Admin, Staff")]
    [HttpPost("quote")]
    public async Task<ActionResult<OrderQuoteDto>> Quote(CreateOrderRequestDto dto)
    {
        return Ok(await _orderService.QuoteAsync(dto));
    }

    // POST/api/orders
    [Authorize(Roles = "Admin, Staff")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequestDto dto)
    {
        try
        {
            var order = await _orderService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById),
                new { id = order.OrderId }, order);
        }
        catch (ValidationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // GET/api/orders
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] OrderStatus.Status? status = null)
    {
        return Ok(await _orderService.GetAllAsync(status));
    }

    // GET/api/orders/{id}
    [Authorize(Roles = "Admin, Staff")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var order = await _orderService.GetByIdAsync(id);

        if (order == null)
            return NotFound(new { message = $"Order {id} was not found." } );

        return Ok(order);
    }

    // PUT/api/orders/{id}/confirm
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}/confirm")]
    public async Task<IActionResult> Confirm(Guid id,
        CancellationToken cancellationToken)
    {
        return await ChangeStatus(id, () => 
        _orderService.ConfirmAsync(id, cancellationToken), 
        "Order confirmed; stock was reduced.", cancellationToken);

    }

    // PUT/api/orders/{id}/complete
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}/complete")]
    public async Task<IActionResult> Complete(Guid id,
        CancellationToken cancellationToken)
    {
        return await ChangeStatus(id, () => 
        _orderService.CompleteAsync(id, cancellationToken), 
        "Order completed.", cancellationToken);

    }

    // PUT/api/orders/{id}/cancel
    [Authorize(Roles = "Admin, Staff")]
    [HttpPut("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(Guid id,
        CancellationToken cancellationToken)
    {
        return await ChangeStatus(id, () =>
        _orderService.CancelAsync(id, cancellationToken), 
        "Pending order cancelled.", cancellationToken);

    }

    private async Task<IActionResult> ChangeStatus(Guid id, Func<Task<bool>> change, string message, CancellationToken cancellationToken)
    {
        if (!await change())
        {
            if (await _orderService.GetByIdAsync(id) == null) return NotFound(new { message = $"Order {id} was not found." });
            return Conflict(new { message = "The order status transition is not allowed, or stock is insufficient to confirm the order." });
        }
        return Ok(new { message, order = await _orderService.GetByIdAsync(id) });
    }

}
