using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetailOrderStockManagementSystem.Backend.Migrations
{
    /// <inheritdoc />
    public partial class BackfillFirstTimeCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE public.customers AS customer
                SET first_time_customer = NOT EXISTS
                (
                    SELECT 1
                    FROM public.orders AS order_record
                    WHERE order_record.customer_id = customer.customer_id
                      AND order_record.status IN
                          ('confirmed'::public.order_status, 'completed'::public.order_status)
                );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // This is a data backfill. Reversing it would overwrite customer status
            // changes made after the migration was applied, so Down intentionally does nothing.
        }
    }
}
