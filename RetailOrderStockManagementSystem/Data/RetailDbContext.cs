using Microsoft.EntityFrameworkCore;
using RetailOrderStockManagementSystem.Models;

namespace RetailOrderStockManagementSystem.Data
{
    public class RetailDbContext : DbContext
    {
        public RetailDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderLineItem> OrderLineItems => Set<OrderLineItem>();
        public DbSet<DiscountRule> DiscountRules => Set<DiscountRule>();
    }
}
