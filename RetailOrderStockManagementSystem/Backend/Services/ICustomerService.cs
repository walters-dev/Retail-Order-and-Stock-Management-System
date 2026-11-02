using RetailOrderStockManagementSystem.Backend.DTOs;

namespace RetailOrderStockManagementSystem.Backend.Services
{
    public interface ICustomerService
    {
        //Create
        Task<CustomerResponseAdminDto> CreateAsync(
            CreateCustomerRequestDto request);

        //Read
        Task<List<CustomerResponseAdminDto>> GetAllAsync();
        Task<CustomerResponseAdminDto?> GetByIdAsync(
            Guid customerId);

        //Update 
        Task<bool> UpdateAsync(
            Guid customerId,
            UpdateCustomerRequestDto request);
        
        // Delete
        Task<bool> DeleteAsync(Guid customerId);
    }
}
