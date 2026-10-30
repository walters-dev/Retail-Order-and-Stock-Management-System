using System.ComponentModel.DataAnnotations;
using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Models;
using RetailOrderStockManagementSystem.Backend.Services;
using RetailOrderStockManagementSystem.Backend.Repositories;

namespace RetailOrderStockManagementSystem.Backend.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;

    public CustomerService(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CustomerResponseAdminDto>> GetAllAsync()
    {
        var customers = await _repository.GetAllAsync();

        return customers.Select(MapToDto).ToList();
    }

    public async Task<CustomerResponseAdminDto?> GetByIdAsync(Guid id)
    {
        var customer = await _repository.GetByIdAsync(id);

        return customer == null ? null : MapToDto(customer);
    }

    public async Task<CustomerResponseAdminDto> CreateAsync(
        CreateCustomerRequestDto dto)
    {
        string mobileNumber = dto.MobileNumber.Trim();

        string? email = string.IsNullOrWhiteSpace(dto.Email)
            ? null
            : dto.Email.Trim();

        if (await _repository.MobileNumberExistsAsync(mobileNumber))
            throw new ValidationException(
                "A customer with this mobile number already exists.");

        if (email != null && await _repository.EmailExistsAsync(email))
            throw new ValidationException(
                "A customer with this email already exists.");

        var newCustomer = new Customer
        {
            FullName = dto.FullName.Trim(),
            MobileNumber = mobileNumber,
            Email = email,
            IsActive = true,
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
            UpdatedAt = null
        };

        var savedCustomer = await _repository.AddAsync(newCustomer);

        return MapToDto(savedCustomer);
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        UpdateCustomerRequestDto dto)
    {
        var customer = await _repository.GetByIdAsync(id);

        if (customer == null) return false;

        string mobileNumber = dto.MobileNumber.Trim();

        string? email = string.IsNullOrWhiteSpace(dto.Email)
            ? null
            : dto.Email.Trim();

        if (await _repository.MobileNumberExistsAsync(mobileNumber, id))
            throw new ValidationException(
                "A customer with this mobile number already exists.");

        if (email != null && await _repository.EmailExistsAsync(email, id))
            throw new ValidationException(
                "A customer with this email already exists.");

        customer.FullName = dto.FullName.Trim();
        customer.MobileNumber = mobileNumber;
        customer.Email = email;

        await _repository.UpdateAsync(customer);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var customer = await _repository.GetByIdAsync(id);

        if (customer == null) return false;

        if (customer.IsActive)
            await _repository.DeleteAsync(customer);

        return true;
    }

    private CustomerResponseAdminDto MapToDto(Customer customer)
    {
        return new CustomerResponseAdminDto
        {
            CustomerId = customer.CustomerId,
            FullName = customer.FullName,
            MobileNumber = customer.MobileNumber,
            Email = customer.Email,
            IsActive = customer.IsActive,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };
    }
}