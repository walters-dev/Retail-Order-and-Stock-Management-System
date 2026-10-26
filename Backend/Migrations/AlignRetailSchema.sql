CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory"(
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;
LOCK TABLE public.customers, public.products, public.orders,
    public.order_line_items, public.discount_rules, public.staff,
    public.admin_managers, public.owner_management
    IN ACCESS EXCLUSIVE MODE;

-- Check your confirmed starting point before changing anything.
-- All tables except discount_rules should be empty.
DO $guard$
DECLARE
    table_name text;
    has_rows boolean;
BEGIN
    FOREACH table_name IN ARRAY ARRAY[
        'customers', 'products', 'orders', 'order_line_items',
        'staff', 'admin_managers', 'owner_management'
    ] LOOP
        EXECUTE format(
            'SELECT EXISTS (SELECT 1 FROM public.%I)',
            table_name
        ) INTO has_rows;

        IF has_rows THEN
            RAISE EXCEPTION
                'Table public.% now contains data. Migration stopped before changing the schema.',
                table_name;
        END IF;
    END LOOP;

    IF to_regclass('public.users') IS NOT NULL THEN
        RAISE EXCEPTION
            'public.users already exists. Review the current schema before applying this first migration.';
    END IF;

    FOREACH table_name IN ARRAY ARRAY[
        'Customers', 'Products', 'Orders',
        'OrderLineItems', 'DiscountRules', 'Users'
    ] LOOP
        IF to_regclass(format('public.%I', table_name)) IS NOT NULL THEN
            RAISE EXCEPTION
                'Unexpected table public.% exists. The earlier draft may have been applied.',
                table_name;
        END IF;
    END LOOP;
END;
$guard$;

-- Create the PostgreSQL enum types.
CREATE TYPE public.order_status AS ENUM (
    'cancelled', 'completed', 'confirmed', 'pending'
);

CREATE TYPE public.user_type AS ENUM (
    'admin', 'manager', 'staff'
);

-- Remove foreign keys temporarily while changing column types.
ALTER TABLE public.orders
    DROP CONSTRAINT fk_orders_customer;

ALTER TABLE public.orders
    DROP CONSTRAINT fk_orders_discount_rule;

ALTER TABLE public.order_line_items
    DROP CONSTRAINT fk_line_items_order;

ALTER TABLE public.order_line_items
    DROP CONSTRAINT fk_line_items_product;

-- Customer IDs must be UUIDs.
-- Already-converted UUID columns keep their type.
DO $customer_ids$
BEGIN
    IF (
        SELECT atttypid
        FROM pg_attribute
        WHERE attrelid = 'public.customers'::regclass
          AND attname = 'customer_id'
          AND NOT attisdropped
    ) <> 'uuid'::regtype THEN
        ALTER TABLE public.customers
            ALTER COLUMN customer_id DROP IDENTITY IF EXISTS;

        ALTER TABLE public.customers
            ALTER COLUMN customer_id DROP DEFAULT;

        ALTER TABLE public.customers
            ALTER COLUMN customer_id TYPE uuid USING NULL::uuid;
    END IF;

    IF (
        SELECT atttypid
        FROM pg_attribute
        WHERE attrelid = 'public.orders'::regclass
          AND attname = 'customer_id'
          AND NOT attisdropped
    ) <> 'uuid'::regtype THEN
        ALTER TABLE public.orders
            ALTER COLUMN customer_id DROP DEFAULT;

        ALTER TABLE public.orders
            ALTER COLUMN customer_id TYPE uuid USING NULL::uuid;
    END IF;
END;
$customer_ids$;

ALTER TABLE public.customers
    ALTER COLUMN customer_id
    SET DEFAULT gen_random_uuid();

-- Convert the retained numeric IDs and foreign keys to integer.
ALTER TABLE public.products
    ALTER COLUMN product_id
    TYPE integer USING product_id::integer;

ALTER TABLE public.discount_rules
    ALTER COLUMN discount_rule_id
    TYPE integer USING discount_rule_id::integer;

ALTER TABLE public.orders
    ALTER COLUMN order_id
    TYPE integer USING order_id::integer,
    ALTER COLUMN discount_rule_id
    TYPE integer USING discount_rule_id::integer;

ALTER TABLE public.order_line_items
    ALTER COLUMN order_line_item_id
    TYPE integer USING order_line_item_id::integer,
    ALTER COLUMN order_id
    TYPE integer USING order_id::integer,
    ALTER COLUMN product_id
    TYPE integer USING product_id::integer;

-- Match the DateOnly and DateOnly? model properties.
ALTER TABLE public.customers
    ALTER COLUMN created_at DROP DEFAULT,
    ALTER COLUMN updated_at DROP DEFAULT;

ALTER TABLE public.customers
    ALTER COLUMN created_at TYPE date USING created_at::date,
    ALTER COLUMN updated_at TYPE date USING updated_at::date,
    ALTER COLUMN updated_at DROP NOT NULL;

ALTER TABLE public.customers
    ALTER COLUMN created_at SET DEFAULT CURRENT_DATE;

ALTER TABLE public.orders
    ALTER COLUMN order_date DROP DEFAULT,
    ALTER COLUMN created_at DROP DEFAULT,
    ALTER COLUMN updated_at DROP DEFAULT,
    ALTER COLUMN order_status DROP DEFAULT;

ALTER TABLE public.orders
    ALTER COLUMN order_date TYPE date USING order_date::date,
    ALTER COLUMN created_at TYPE date USING created_at::date,
    ALTER COLUMN updated_at TYPE date USING updated_at::date,
    ALTER COLUMN updated_at DROP NOT NULL,
    ALTER COLUMN order_status TYPE public.order_status
        USING lower(order_status::text)::public.order_status;

ALTER TABLE public.orders
    ALTER COLUMN order_date SET DEFAULT CURRENT_DATE,
    ALTER COLUMN created_at SET DEFAULT CURRENT_DATE,
    ALTER COLUMN order_status
        SET DEFAULT 'pending'::public.order_status;

-- These properties already exist in your Product model.
ALTER TABLE public.products
    ADD COLUMN created_at date NOT NULL DEFAULT CURRENT_DATE,
    ADD COLUMN updated_at date;

-- Adapt staff into the single application users table.
ALTER TABLE public.staff RENAME TO users;

ALTER TABLE public.users
    RENAME COLUMN staff_id TO user_id;

ALTER TABLE public.users
    ALTER COLUMN user_id DROP IDENTITY IF EXISTS;

ALTER TABLE public.users
    ALTER COLUMN user_id DROP DEFAULT;

ALTER TABLE public.users
    ALTER COLUMN user_id TYPE uuid USING NULL::uuid;

ALTER TABLE public.users
    ALTER COLUMN user_id SET DEFAULT gen_random_uuid();

ALTER TABLE public.users
    ADD COLUMN user_type public.user_type
    NOT NULL DEFAULT 'staff'::public.user_type;

ALTER TABLE public.users
    RENAME CONSTRAINT staff_pkey TO users_pkey;

-- Remove the two role tables you no longer need.
DROP TABLE public.admin_managers;
DROP TABLE public.owner_management;

-- Represent uniqueness using the indexes configured in EF.
ALTER TABLE public.customers
    DROP CONSTRAINT customers_mobile_number_key;

ALTER TABLE public.customers
    DROP CONSTRAINT customers_email_key;

ALTER TABLE public.products
    DROP CONSTRAINT products_sku_key;

ALTER TABLE public.users
    DROP CONSTRAINT staff_email_key;

ALTER TABLE public.users
    DROP CONSTRAINT staff_username_key;

CREATE UNIQUE INDEX customers_mobile_number_key
    ON public.customers (mobile_number);

CREATE UNIQUE INDEX customers_email_key
    ON public.customers (email);

CREATE UNIQUE INDEX products_sku_key
    ON public.products (sku);

CREATE UNIQUE INDEX users_email_key
    ON public.users (email);

CREATE UNIQUE INDEX users_username_key
    ON public.users (username);

-- Restore the relationships and their original delete rules.
ALTER TABLE public.orders
    ADD CONSTRAINT fk_orders_customer
    FOREIGN KEY (customer_id)
    REFERENCES public.customers (customer_id)
    ON DELETE RESTRICT;

ALTER TABLE public.orders
    ADD CONSTRAINT fk_orders_discount_rule
    FOREIGN KEY (discount_rule_id)
    REFERENCES public.discount_rules (discount_rule_id)
    ON DELETE SET NULL;

ALTER TABLE public.order_line_items
    ADD CONSTRAINT fk_line_items_order
    FOREIGN KEY (order_id)
    REFERENCES public.orders (order_id)
    ON DELETE CASCADE;

ALTER TABLE public.order_line_items
    ADD CONSTRAINT fk_line_items_product
    FOREIGN KEY (product_id)
    REFERENCES public.products (product_id)
    ON DELETE RESTRICT;

CREATE INDEX ix_orders_customer_id
    ON public.orders (customer_id);

CREATE INDEX ix_orders_discount_rule_id
    ON public.orders (discount_rule_id);

CREATE INDEX ix_line_items_order_id
    ON public.order_line_items (order_id);

CREATE INDEX ix_line_items_product_id
    ON public.order_line_items (product_id);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20261026125614_AlignRetailSchema', '10.0.13');

COMMIT;

