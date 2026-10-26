using Microsoft.EntityFrameworkCore;
using RetailOrderStockManagementSystem.Backend.Models;
using OrderState = RetailOrderStockManagementSystem.Backend.Enums.OrderStatus.Status;
using UserRole = RetailOrderStockManagementSystem.Backend.Enums.Users.UserTypes;

namespace RetailOrderStockManagementSystem.Backend.Data
{
    public class RetailDbContext : DbContext
    {
        public RetailDbContext(DbContextOptions<RetailDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLineItem> OrderLineItems { get; set; }
        public DbSet<DiscountRule> DiscountRules { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // CUSTOMERS
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("customers", "public");

                entity.HasKey(x => x.CustomerId)
                    .HasName("customers_pkey");

                entity.Property(x => x.CustomerId)
                    .HasColumnName("customer_id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(x => x.FullName)
                    .HasColumnName("full_name")
                    .IsRequired();

                entity.Property(x => x.MobileNumber)
                    .HasColumnName("mobile_number")
                    .IsRequired();

                entity.Property(x => x.Email)
                    .HasColumnName("email")
                    .IsRequired(false);

                entity.Property(x => x.IsActive)
                    .HasColumnName("is_active")
                    .HasDefaultValue(true);

                entity.Property(x => x.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_DATE");

                entity.Property(x => x.UpdatedAt)
                    .HasColumnName("updated_at");

                entity.HasIndex(x => x.MobileNumber)
                    .IsUnique()
                    .HasDatabaseName("customers_mobile_number_key");

                entity.HasIndex(x => x.Email)
                    .IsUnique()
                    .HasDatabaseName("customers_email_key");
            });

            // PRODUCTS
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products", "public", table =>
                {
                    table.HasCheckConstraint(
                        "chk_product_price",
                        "unit_price >= 0");

                    table.HasCheckConstraint(
                        "chk_stock_quantity",
                        "stock_quantity >= 0");
                });

                entity.HasKey(x => x.ProductId)
                    .HasName("products_pkey");

                entity.Property(x => x.ProductId)
                    .HasColumnName("product_id")
                    .UseIdentityByDefaultColumn();

                entity.Property(x => x.SKU)
                    .HasColumnName("sku")
                    .HasMaxLength(8)
                    .IsRequired();

                entity.Property(x => x.Name)
                    .HasColumnName("name")
                    .IsRequired();

                entity.Property(x => x.Category)
                    .HasColumnName("category")
                    .IsRequired();

                entity.Property(x => x.UnitPrice)
                    .HasColumnName("unit_price")
                    .HasPrecision(12, 2);

                entity.Property(x => x.StockQuantity)
                    .HasColumnName("stock_quantity")
                    .HasDefaultValue(0);

                entity.Property(x => x.IsActive)
                    .HasColumnName("is_active")
                    .HasDefaultValue(true);

                entity.Property(x => x.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_DATE");

                entity.Property(x => x.UpdatedAt)
                    .HasColumnName("updated_at");

                entity.HasIndex(x => x.SKU)
                    .IsUnique()
                    .HasDatabaseName("products_sku_key");
            });

            // DISCOUNT RULES
            modelBuilder.Entity<DiscountRule>(entity =>
            {
                entity.ToTable("discount_rules", "public", table =>
                {
                    table.HasCheckConstraint(
                        "chk_discount_percent",
                        "percent >= 0 AND percent <= 100");

                    table.HasCheckConstraint(
                        "chk_minimum_order_amount",
                        "minimum_order_amount >= 0");
                });

                entity.HasKey(x => x.DiscountRuleId)
                    .HasName("discount_rules_pkey");

                entity.Property(x => x.DiscountRuleId)
                    .HasColumnName("discount_rule_id")
                    .UseIdentityByDefaultColumn();

                entity.Property(x => x.Name)
                    .HasColumnName("name")
                    .IsRequired();

                entity.Property(x => x.Percent)
                    .HasColumnName("percent")
                    .HasPrecision(5, 2);

                entity.Property(x => x.MinimumOrderAmount)
                    .HasColumnName("minimum_order_amount")
                    .HasPrecision(12, 2)
                    .HasDefaultValue(0m);

                entity.Property(x => x.IsActive)
                    .HasColumnName("is_active")
                    .HasDefaultValue(true);
            });

            // ORDERS
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("orders", "public", table =>
                {
                    table.HasCheckConstraint(
                        "chk_order_subtotal",
                        "subtotal >= 0");

                    table.HasCheckConstraint(
                        "chk_order_discount_amount",
                        "discount_amount >= 0");

                    table.HasCheckConstraint(
                        "chk_order_final_total",
                        "final_total >= 0");
                });

                entity.HasKey(x => x.OrderId)
                    .HasName("orders_pkey");

                entity.Property(x => x.OrderId)
                    .HasColumnName("order_id")
                    .UseIdentityByDefaultColumn();

                entity.Property(x => x.CustomerId)
                    .HasColumnName("customer_id");

                entity.Property(x => x.OrderDate)
                    .HasColumnName("order_date")
                    .HasDefaultValueSql("CURRENT_DATE");

                entity.Property(x => x.Subtotal)
                    .HasColumnName("subtotal")
                    .HasPrecision(12, 2)
                    .HasDefaultValue(0m);

                entity.Property(x => x.DiscountAmount)
                    .HasColumnName("discount_amount")
                    .HasPrecision(12, 2)
                    .HasDefaultValue(0m);

                entity.Property(x => x.FinalTotal)
                    .HasColumnName("final_total")
                    .HasPrecision(12, 2)
                    .HasDefaultValue(0m);

                entity.Property(x => x.Status)
                    .HasColumnName("order_status")
                    .HasDefaultValue(OrderState.Pending);

                entity.Property(x => x.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_DATE");

                entity.Property(x => x.UpdatedAt)
                    .HasColumnName("updated_at");

                entity.Property(x => x.DiscountRuleId)
                    .HasColumnName("discount_rule_id");

                entity.HasOne<Customer>()
                    .WithMany()
                    .HasForeignKey(x => x.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_orders_customer");

                entity.HasOne<DiscountRule>()
                    .WithMany()
                    .HasForeignKey(x => x.DiscountRuleId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_orders_discount_rule");

                entity.HasIndex(x => x.CustomerId)
                    .HasDatabaseName("ix_orders_customer_id");

                entity.HasIndex(x => x.DiscountRuleId)
                    .HasDatabaseName("ix_orders_discount_rule_id");
            });

            // ORDER LINE ITEMS
            modelBuilder.Entity<OrderLineItem>(entity =>
            {
                entity.ToTable("order_line_items", "public", table =>
                {
                    table.HasCheckConstraint(
                        "chk_line_item_quantity",
                        "quantity > 0");
                });

                entity.HasKey(x => x.OrderLineItemId)
                    .HasName("order_line_items_pkey");

                entity.Property(x => x.OrderLineItemId)
                    .HasColumnName("order_line_item_id")
                    .UseIdentityByDefaultColumn();

                entity.Property(x => x.ProductId)
                    .HasColumnName("product_id");

                entity.Property(x => x.OrderId)
                    .HasColumnName("order_id");

                entity.Property(x => x.Quantity)
                    .HasColumnName("quantity");

                entity.Property(x => x.LineTotal)
                    .HasColumnName("line_total")
                    .HasPrecision(12, 2);

                entity.HasOne<Order>()
                    .WithMany()
                    .HasForeignKey(x => x.OrderId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_line_items_order");

                entity.HasOne<Product>()
                    .WithMany()
                    .HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_line_items_product");

                entity.HasIndex(x => x.OrderId)
                    .HasDatabaseName("ix_line_items_order_id");

                entity.HasIndex(x => x.ProductId)
                    .HasDatabaseName("ix_line_items_product_id");
            });

            // USERS
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users", "public");

                entity.HasKey(x => x.UserId)
                    .HasName("users_pkey");

                entity.Property(x => x.UserId)
                    .HasColumnName("user_id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(x => x.Email)
                    .HasColumnName("email")
                    .IsRequired();

                entity.Property(x => x.Username)
                    .HasColumnName("username")
                    .HasMaxLength(50)
                    .IsRequired();

                // Maps your current property to the existing database column.
                // The application must hash the password before storing it.
                entity.Property(x => x.Password)
                    .HasColumnName("password_hash")
                    .IsRequired();

                entity.Property(x => x.Type)
                    .HasColumnName("user_type")
                    .HasDefaultValue(UserRole.Staff);

                entity.HasIndex(x => x.Email)
                    .IsUnique()
                    .HasDatabaseName("users_email_key");

                entity.HasIndex(x => x.Username)
                    .IsUnique()
                    .HasDatabaseName("users_username_key");
            });
        }
    }
}