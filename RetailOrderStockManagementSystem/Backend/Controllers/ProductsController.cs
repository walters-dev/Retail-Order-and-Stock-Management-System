using Microsoft.AspNetCore.Mvc;
using RetailOrderStockManagementSystem.Backend.Models;
using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Services;
using RetailOrderStockManagementSystem.Backend.Services.Interfaces;

namespace RetailOrderStockManagementSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    // Create a private readonly field for the interface
    private readonly IProductService _productService;

    // Request the interface in the constructor
    public ProductsController(IProductService productService)
    {
        _productService = productService; // Assign it to our field
    }

    //private readonly IProductService _productService;

    //public ProductsController(IProductService productService)
    //{
    //    _productService = productService;
    //}

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if(product == null)
        {
            return NotFound();
        }
        
        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductRequestDto dto)
    {
        var product = await _productService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateProductRequestDto dto)
    {
        var updated = await _productService.UpdateAsync(id, dto);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _productService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}