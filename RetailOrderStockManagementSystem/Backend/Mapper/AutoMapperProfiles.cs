using AutoMapper;
using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Enums;
using RetailOrderStockManagementSystem.Backend.Models;
using System.Runtime.InteropServices;

namespace RetailOrderStockManagementSystem.Backend.Mapper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<CustomerResponseAdminDto, Customer>().ReverseMap();
            CreateMap<Customer, CreateCustomerRequestDto>().ReverseMap();
            CreateMap<Customer, UpdateCustomerRequestDto>().ReverseMap();
            CreateMap<CustomerResponseStaffDto, Customer>().ReverseMap();
            CreateMap<Product, CreateProductRequestDto>().ReverseMap();
            CreateMap<Product, UpdateProductRequestDto>().ReverseMap();
            CreateMap<ProductResponseAdminDto, Product>().ReverseMap();
            CreateMap<OrderResponseAdminDto, Order>().ReverseMap();
            CreateMap<OrderResponseStaffDto, Order>().ReverseMap();
            CreateMap<User, CreateUserRequestDto>().ReverseMap();
            CreateMap<User, LogInRequestDto>().ReverseMap();
        }
    }
}
