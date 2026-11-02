using RetailOrderStockManagementSystem.Backend.Models;

namespace RetailOrderStockManagementSystem.Backend.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer> AddAsync(Customer customer);

        Task<List<Customer>> GetAllAsync();

        Task<Customer?> GetByIdAsync(Guid customerId);

        Task UpdateAsync(Customer customer);

        Task DeleteAsync(Customer customer);

        Task<bool> MobileNumberExistsAsync(
            string mobileNumber,
            Guid? excludeCustomerId = null);

        Task<bool> EmailExistsAsync(
            string email,
            Guid? excludeCustomerId = null);
    }
}