using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RetailOrderStockManagementSystem.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderReceiptIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "receipt_id",
                schema: "public",
                table: "orders",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "orders_receipt_id_key",
                schema: "public",
                table: "orders",
                column: "receipt_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "orders_receipt_id_key",
                schema: "public",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "receipt_id",
                schema: "public",
                table: "orders");
        }
    }
}
