using Microsoft.EntityFrameworkCore;
using RetailOrderStockManagementSystem.Backend.Data;
using RetailOrderStockManagementSystem.Backend.Models;

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
        public async Task<List<Customer>> GetAllAsync()
        {
            return await _context.Customers
                .AsNoTracking()
                .OrderBy(customer => customer.FullName)
                .ThenBy(customer => customer.CustomerId)
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