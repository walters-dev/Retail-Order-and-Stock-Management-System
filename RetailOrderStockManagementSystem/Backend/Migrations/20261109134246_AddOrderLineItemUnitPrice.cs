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
            migrationBuilder.AddColumn<decimal>(
                name: "unit_price",
                schema: "public",
                table: "order_line_items",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "unit_price",
                schema: "public",
                table: "order_line_items");
        }
    }
}
