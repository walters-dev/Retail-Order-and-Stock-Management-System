using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Models;

namespace RetailOrderStockManagementSystem.Backend.Services;

public class DiscountService
{
    private const decimal LargeOrderDiscountPercentage = 0.10m;
    private const decimal FirstTimeCustomerDiscountPercentage = 0.15m;
    private const decimal LoyaltyDiscountPercentage = 0.05m;

    public DiscountResult CalculateDiscount(Customer customer, Order order, 
        decimal subtotal)
    {
        decimal highestPercentage = 0m;

        if (order.LargeOrder && 
            LargeOrderDiscountPercentage > highestPercentage)
        {
            return new DiscountResult
            {
                DiscountName = "Large Order Discount",
                DiscountAmount = subtotal * LargeOrderDiscountPercentage
            };
        }

        if (customer.FirstTimeCustomer && 
            FirstTimeCustomerDiscountPercentage > highestPercentage)
        {
            return new DiscountResult
            {
                DiscountName = "First-Time Customer Discount",
                DiscountAmount = subtotal * FirstTimeCustomerDiscountPercentage
            };
        }

        if (customer.LoyalCustomer &&
            LoyaltyDiscountPercentage > highestPercentage)
        {
            return new DiscountResult
            {
                DiscountName = "Loyal Customer Discount",
                DiscountAmount = subtotal * LoyaltyDiscountPercentage
            };
        }

        return new DiscountResult
        {
            DiscountName = "No discount",
            DiscountAmount = 0m
        };
    }









}
