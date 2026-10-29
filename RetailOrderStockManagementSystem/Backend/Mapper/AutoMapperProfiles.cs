using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Models;
using System.Runtime.InteropServices;
using AutoMapper;

namespace RetailOrderStockManagementSystem.Backend.Mapper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Customer, CustomerResponseAdminDto>();
            CreateMap<Customer, CreateCustomerRequestDto>();
            CreateMap<Customer, UpdateCustomerRequestDto>();
            CreateMap<Customer, CustomerResponseStaffDto>();
            CreateMap<Product, CreateProductRequestDto>();
            CreateMap<Product, UpdateProductRequestDto>();
            CreateMap<Product, ProductResponseAdminDto>();
            CreateMap<Order, OrderResponseAdminDto>();
            CreateMap<Order, OrderResponseStaffDto>();
            CreateMap<User, CreateUserRequestDto>();
            //do i need a user response sign and request?
        }
    }
}
