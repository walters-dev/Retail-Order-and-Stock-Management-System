using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Services;
using System.ComponentModel.DataAnnotations;

namespace RetailOrderStockManagementSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // POST/api/products
    [HttpPost]
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
    public async Task<IActionResult> GetAll([FromQuery] ProductQueryParameters parameters)
    {
        var products = await _productService.GetAllAsync(parameters);
        return Ok(products);
    }

    // GET/api/products/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product == null)
            return NotFound(new { message = $"Product with ID {id} was not found." });

        return Ok(product);
    }

    // PUT/api/products/{id}
    [HttpPut("{id:int}")]
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
    public async Task<IActionResult> Delete(int id, [FromBody] bool isActive)
    {
        var updatedProduct = 
            await _productService.UpdateActiveStatusAsync(id, isActive);

        if (updatedProduct == null)
            return NotFound(new { message = $"Product with ID {id} was not found." });

        return Ok(updatedProduct);
    }
}
