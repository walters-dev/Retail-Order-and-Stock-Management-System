using RetailOrderStockManagementSystem.Backend.DTOs;
using RetailOrderStockManagementSystem.Backend.Models;

namespace RetailOrderStockManagementSystem.Backend.Services;

public class DiscountService
{
    //private const decimal LargeOrderDiscountPercentage = 0.10m; // get from DB
    //private const decimal FirstTimeCustomerDiscountPercentage = 0.15m; // get from DB
    //private const decimal LoyaltyDiscountPercentage = 0.05m; // get from DB

    //public DiscountResult CalculateDiscount(Customer customer, Order order,
    //    decimal subtotal)
    //{
    //    decimal highestPercentage = 0m;
    //    string discountName = "No discount";

    //    if (subtotal > 2000 &&
    //        LargeOrderDiscountPercentage > highestPercentage)
    //    {
    //        highestPercentage = LargeOrderDiscountPercentage;
    //        discountName = "Large Order Discount";
    //    }

    //    if (customer.FirstTimeCustomer &&
    //        FirstTimeCustomerDiscountPercentage > highestPercentage)
    //    {
    //        highestPercentage = FirstTimeCustomerDiscountPercentage;
    //        discountName = "First-Time Customer Discount";
    //    }

    //    if (customer.LoyalCustomer &&
    //        LoyaltyDiscountPercentage > highestPercentage)
    //    {
    //        highestPercentage = LoyaltyDiscountPercentage;
    //        discountName = "Loyal Customer Discount";
    //    }

    //    return new DiscountResult
    //    {
    //        DiscountName = discountName,
    //        DiscountPercentage = highestPercentage,
    //        DiscountAmount = subtotal * highestPercentage
    //    };
    //}
}
