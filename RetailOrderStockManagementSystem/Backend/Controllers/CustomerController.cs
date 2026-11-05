using k8s.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Repositories;
using RetailOrderStockManagementSystem.Backend.Services;
using System.ComponentModel.DataAnnotations;
namespace RetailOrderStockManagementSystem.Backend.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        //GET /api/customers
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery]CustomerQueryParameters parameters)
        {
            var customers = await _customerService.GetAllAsync(parameters);
            return Ok(customers);
        } 

        //GET /api/customers/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null) return NotFound();
            return Ok(customer);
        }

        // POST /api/customers
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerRequestDto dto)
        {
            try
            {
                var customer = await _customerService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = customer.CustomerId },
                    customer);
            }
            catch (ValidationException exception)
            {
                return Conflict(new { message = "A customer with this email or mobile number already exists." });
            }
            
        }

        // PUT /api/customers/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id,[FromBody] UpdateCustomerRequestDto dto)
        {
            try
            {
                bool updated = await _customerService.UpdateAsync(id, dto);

                if (!updated) return NotFound();

                return NoContent();
            }
            catch (ValidationException exception)
            {
                return Conflict(new { message = "A customer with this email or mobile number already exists." });
            }
            
        }

        // DELETE /api/customers/{id}
        [HttpPut("{id:guid}/isActive")]
        public async Task<IActionResult> Delete(Guid id)
        {
            bool deleted = await _customerService.DeleteAsync(id);

            if (!deleted) return NotFound();

            var customer = await _customerService.GetByIdAsync(id);

            return Ok(customer);
        }

    }
}
