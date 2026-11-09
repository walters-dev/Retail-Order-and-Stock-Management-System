using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using OpenTelemetry.Trace;
using RetailOrderStockManagementSystem.Backend.Models;

#nullable disable

namespace RetailOrderStockManagementSystem.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerDiscountFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "first_time_customer_discount",
                schema: "public",
                table: "customers",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "loyalty_customer_discount",
                schema: "public",
                table: "customers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
            //now i need a migration for the unit price within the order line table and discount percentage within the orders table
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "first_time_customer_discount",
                schema: "public",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "loyalty_customer_discount",
                schema: "public",
                table: "customers");
        }
    }
}
