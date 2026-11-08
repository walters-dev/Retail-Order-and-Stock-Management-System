using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetailOrderStockManagementSystem.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderLineItemUnitPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'public'
                          AND table_name = 'order_line_items'
                          AND column_name = 'unit_price') THEN
                        ALTER TABLE public.order_line_items
                            ADD COLUMN unit_price numeric(12,2) NOT NULL DEFAULT 0;
                    END IF;

                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'public'
                          AND table_name = 'customers'
                          AND column_name = 'first_time_customer') THEN
                        ALTER TABLE public.customers
                            ADD COLUMN first_time_customer boolean NOT NULL DEFAULT false;
                    END IF;

                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'public'
                          AND table_name = 'customers'
                          AND column_name = 'loyal_customer') THEN
                        ALTER TABLE public.customers
                            ADD COLUMN loyal_customer boolean NOT NULL DEFAULT false;
                    END IF;
                END $$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE public.order_line_items DROP COLUMN IF EXISTS unit_price;
                ALTER TABLE public.customers DROP COLUMN IF EXISTS first_time_customer;
                ALTER TABLE public.customers DROP COLUMN IF EXISTS loyal_customer;
                """);
        }
    }
}
