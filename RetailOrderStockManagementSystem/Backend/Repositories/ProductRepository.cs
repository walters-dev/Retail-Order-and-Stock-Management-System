using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore;
//using RetailOrderStockManagementSystem.Backend.Data;
using RetailOrderStockManagementSystem.Backend.Models;
using RetailOrderStockManagementSystem.Backend.Repositories.Interfaces;
using RetailOrderStockManagementSystem.Backend.Services.Interfaces;

namespace RetailOrderStockManagementSystem.Backend.Repositories;

public class ProductRepository : IProductRepository
{
    // Create a private readonly field for the interface
    private readonly IProductService _productService;

    // Request the interface in the constructor
    public ProductRepository(IProductService productService)
    {
        _productService = productService; // Assign it to our field
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _productService.GetAllAsync();
    }














}