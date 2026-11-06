using Microsoft.EntityFrameworkCore;
using RetailOrderStockManagementSystem.Backend.Data;
using RetailOrderStockManagementSystem.Backend.Models;
using RetailOrderStockManagementSystem.Backend.DTOs;

namespace RetailOrderStockManagementSystem.Backend.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly RetailDbContext _context;

        public CustomerRepository(RetailDbContext context)
        {
            _context = context;
        }

        // CREATE
        public async Task<Customer> AddAsync(Customer customer)
        {
            _context.Customers.Add(customer);

            await _context.SaveChangesAsync();

            return customer;
        }

        // READ: includes active and inactive customers.
        public async Task<List<Customer>> GetAllAsync(
      CustomerQueryParameters? parameters = null)
        {
            if (parameters == null)
                parameters = new CustomerQueryParameters();

            var query = _context.Customers.AsNoTracking();

            // Search by name, mobile number or email.
            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                string search = parameters.Search.Trim().ToLower();

                query = query.Where(customer =>
                    customer.FullName.ToLower().Contains(search) ||
                    customer.MobileNumber.Contains(search) ||
                    (customer.Email != null &&
                     customer.Email.ToLower().Contains(search)));
            }

            // Filter only when IsActive was supplied.
            if (parameters.IsActive.HasValue)
            {
                bool isActive = parameters.IsActive.Value;

                query = query.Where(customer =>
                    customer.IsActive == isActive);
            }

            // Calculate how many customers to skip.
            long skip = ((long)parameters.Page - 1) * parameters.PageSize;

            if (skip > int.MaxValue)
                return new List<Customer>();

            return await query
                .OrderBy(customer => customer.FullName)
                .ThenBy(customer => customer.CustomerId)
                .Skip((int)skip)
                .Take(parameters.PageSize)
                .ToListAsync();
        }

        // READ: returns null when the customer does not exist.
        public async Task<Customer?> GetByIdAsync(Guid customerId)
        {
            return await _context.Customers.FindAsync(customerId);
        }

        // UPDATE
        public async Task UpdateAsync(Customer customer)
        {
            customer.UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow);

            _context.Customers.Update(customer);

            await _context.SaveChangesAsync();
        }

        // DELETE: deactivates the customer.
        public async Task DeleteAsync(Customer customer)
        {
            customer.IsActive = false;

            await UpdateAsync(customer);
        }

        //Check If Mobile Number Exists
        public async Task<bool> MobileNumberExistsAsync(
            string mobileNumber,
            Guid? excludeCustomerId = null)
        {
            var query = _context.Customers
                .Where(customer => customer.MobileNumber == mobileNumber);

            if (excludeCustomerId.HasValue)
            {
                Guid customerId = excludeCustomerId.Value;

                query = query.Where(customer => customer.CustomerId != customerId);
            }

            return await query.AnyAsync();
        }

        //Check if Email Exists
        public async Task<bool> EmailExistsAsync(
            string email,
            Guid? excludeCustomerId = null)
        {
            var query = _context.Customers.Where(customer => customer.Email == email);

            if (excludeCustomerId.HasValue)
            {
                Guid customerId = excludeCustomerId.Value;

                query = query.Where(customer => customer.CustomerId != customerId);
            }

            return await query.AnyAsync();
        }
    }
}