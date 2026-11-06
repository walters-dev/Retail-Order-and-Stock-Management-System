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
        catch (ValidationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        //catch (DbUpdateException ex) when
        //        (ex.InnerException is PostgresException postgres && postgres.SqlState
        //        == PostgresErrorCodes.UniqueViolation &&
        //        (postgres.ConstraintName == "products_sku"))
        //{
        //    return Conflict(
        //        new { message = "A product with this SKU already exists." });
        //}          
    }

    // GET/api/products
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProductQueryParameters parameters)
    {
        var products = await _productService.GetAllAsync(parameters);
        return Ok(products);
    }

    // GET/api/products/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product == null)
            return NotFound();

        return Ok(product);
    }

    // PUT/api/products/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequestDto dto)
    {
        try
        {
            var updatedProduct = await _productService.UpdateAsync(dto, id);
            return updatedProduct ? NoContent() : NotFound();
        }
        catch (ValidationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (DbUpdateException ex) when
        (ex.InnerException is PostgresException postgres && postgres.SqlState
        == PostgresErrorCodes.UniqueViolation && 
        (postgres.ConstraintName == "product_sku"))
        {
            return Conflict( 
                new { message = "A product with this sku already exists."});
        }
    }

    // DELETE/api/products/{id}
    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        var isProduct = await _productService.DeleteAsync(id);
        return isProduct ? NoContent() : NotFound();
    }
}
