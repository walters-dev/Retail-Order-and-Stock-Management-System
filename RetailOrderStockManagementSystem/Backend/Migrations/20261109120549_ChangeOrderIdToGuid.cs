using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RetailOrderStockManagementSystem.Backend.Migrations
{
    /// <inheritdoc />
    public partial class ChangeOrderIdToGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
        -- Give each existing order a UUID and copy it to its line items.
        ALTER TABLE public.orders
            ADD COLUMN new_order_id uuid NOT NULL DEFAULT gen_random_uuid();

        ALTER TABLE public.order_line_items
            ADD COLUMN new_order_id uuid;

        UPDATE public.order_line_items AS item
        SET new_order_id = o.new_order_id
        FROM public.orders AS o
        WHERE item.order_id = o.order_id;

        ALTER TABLE public.order_line_items
            ALTER COLUMN new_order_id SET NOT NULL;

        -- Remove the foreign key while changing the types on both sides.
        ALTER TABLE public.order_line_items
            DROP CONSTRAINT fk_line_items_order;

        ALTER TABLE public.orders ALTER COLUMN order_id DROP IDENTITY IF EXISTS;
        ALTER TABLE public.orders ALTER COLUMN order_id DROP DEFAULT;
        ALTER TABLE public.orders ALTER COLUMN order_id TYPE uuid USING new_order_id;
        ALTER TABLE public.orders ALTER COLUMN order_id SET DEFAULT gen_random_uuid();

        ALTER TABLE public.order_line_items
            ALTER COLUMN order_id TYPE uuid USING new_order_id;

        ALTER TABLE public.order_line_items
            ADD CONSTRAINT fk_line_items_order
            FOREIGN KEY (order_id) REFERENCES public.orders (order_id)
            ON DELETE CASCADE;

        ALTER TABLE public.order_line_items DROP COLUMN new_order_id;
        ALTER TABLE public.orders DROP COLUMN new_order_id;
        """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new NotSupportedException(
                "The original integer order IDs cannot be restored automatically.");
        }
    }
}
