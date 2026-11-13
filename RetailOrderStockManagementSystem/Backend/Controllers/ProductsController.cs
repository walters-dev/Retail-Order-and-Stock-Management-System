using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Services;
using System.ComponentModel.DataAnnotations;

namespace RetailOrderStockManagementSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin, Staff")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // POST/api/products
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateProductRequestDto dto)
    {
        try
        {
            var createdProduct = await _productService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = createdProduct.ProductId }, createdProduct);
        }
        catch (ArgumentException ex)
        {
            return Conflict(new { message = ex.Message });
        }         
    }

    // GET/api/products
    [HttpGet]
    [Authorize(Roles = "Admin, Staff")]
    public async Task<IActionResult> GetAll([FromQuery] ProductQueryParameters parameters)
    {
        if (User.IsInRole("Admin"))
            return Ok(await _productService.GetAllAsync(parameters));

        // Staff can view sellable products only and cannot request inactive records.
        parameters.IncludeInactive = false;
        return Ok(await _productService.GetAllForStaffAsync(parameters));
    }

    // GET/api/products/{id}
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin, Staff")]
    public async Task<IActionResult> GetById(int id)
    {
        if (User.IsInRole("Admin"))
        {
            var adminProduct = await _productService.GetByIdAsync(id);

            if (adminProduct == null)
                return NotFound(new { message = $"Product with ID {id} was not found." });

            return Ok(adminProduct);
        }

        var product = await _productService.GetByIdForStaffAsync(id);

        if (product == null)
            return NotFound(new { message = $"Product with ID {id} was not found." });

        return Ok(product);
    }

    // PUT/api/products/{id}
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequestDto dto)
    {
        try
        {
            bool updatedProduct = await _productService.UpdateAsync(dto, id);

            if (!updatedProduct)
                return NotFound(new { message = $"Product with ID {id} was not found." });

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // PUT/api/products/{id}/isActive
    [HttpPut("{id:int}/isActive")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, [FromBody] bool isActive)
    {
        var updatedProduct = 
            await _productService.UpdateActiveStatusAsync(id, isActive);

        if (updatedProduct == null)
            return NotFound(new { message = $"Product with ID {id} was not found." });

        return Ok(updatedProduct);
    }
}
