import { useMemo, useState } from "react";
import "./Products.css";

const LOW_STOCK_THRESHOLD = 50;

function normalizeProduct(raw, index) {
  const safeRaw = raw && typeof raw === "object" ? raw : {};

  return {
    productId: safeRaw.productId ?? safeRaw.id ?? index + 1,
    sku: String(safeRaw.sku ?? safeRaw.SKU ?? ""),
    name: String(safeRaw.name ?? safeRaw.productName ?? ""),
    category: String(safeRaw.category ?? "General"),
    unitPrice: Number(safeRaw.unitPrice ?? safeRaw.price ?? 0),
    stockQuantity: Number(safeRaw.stockQuantity ?? safeRaw.stock ?? 0),
    isActive: Boolean(safeRaw.isActive ?? safeRaw.active ?? true),
  };
}

export default function Products({
  products = [],
  loading = false,
  error = "",
  onRefresh,
}) {
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedCategory, setSelectedCategory] = useState("ALL");

  // Step A: Normalize every product item once per change to `products`
  const normalizedProducts = useMemo(
    () => products.map((item, idx) => normalizeProduct(item, idx)),
    [products]
  );

  // Step B: Build a sorted list of unique categories for the dropdown
  const categories = useMemo(() => {
    const unique = new Set(
      normalizedProducts.map((product) => product.category).filter(Boolean)
    );
    return [...unique].sort((a, b) => a.localeCompare(b));
  }, [normalizedProducts]);

  // Step C: Filter by selected category and search text (SKU or product name)
  const filteredProducts = useMemo(() => {
    const query = searchTerm.trim().toLowerCase();

    return normalizedProducts.filter((product) => {
      const matchesCategory =
        selectedCategory === "ALL" || product.category === selectedCategory;

      const matchesSearch =
        !query ||
        product.sku.toLowerCase().includes(query) ||
        product.name.toLowerCase().includes(query);

      return matchesCategory && matchesSearch;
    });
  }, [normalizedProducts, searchTerm, selectedCategory]);

  // Summary counts for the subtitle
  const activeCount = normalizedProducts.filter((p) => p.isActive).length;
  const lowStockCount = normalizedProducts.filter(
    (p) => p.isActive && p.stockQuantity <= LOW_STOCK_THRESHOLD
  ).length;

  return (
    <section className="products-screen" aria-label="Products">
      <div className="products-header">
        <div>
          <h2 className="products-title">Products</h2>
          <p className="products-subtitle">
            Active products and current stock ({activeCount} active &bull;{" "}
            {lowStockCount} low stock)
          </p>
        </div>

        {onRefresh && (
          <button
            type="button"
            className="products-refresh-btn"
            onClick={onRefresh}
            disabled={loading}
          >
            {loading ? "Refreshing..." : "Refresh"}
          </button>
        )}
      </div>

      <div className="products-toolbar">
        <input
          type="search"
          className="products-search-input"
          placeholder="Search by SKU or name"
          aria-label="Search by SKU or name"
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
        />

        <select
          className="products-category-select"
          aria-label="Filter by category"
          value={selectedCategory}
          onChange={(e) => setSelectedCategory(e.target.value)}
        >
          <option value="ALL">All categories</option>
          {categories.map((category) => (
            <option key={category} value={category}>
              {category}
            </option>
          ))}
        </select>
      </div>

      {loading && (
        <p className="products-status-message">Loading products...</p>
      )}

      {!loading && error && (
        <div className="products-error-banner" role="alert">
          {error}
        </div>
      )}

      {!loading && !error && (
        <div className="products-table-wrapper">
          <table className="products-table">
            <thead>
              <tr>
                <th>SKU</th>
                <th>Product</th>
                <th>Category</th>
                <th>Price</th>
                <th>Stock</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              {filteredProducts.length === 0 ? (
                <tr>
                  <td colSpan={6} className="products-empty-cell">
                    No products match your search or category filter.
                  </td>
                </tr>
              ) : (
                filteredProducts.map((product) => {
                  const isLowStock = product.stockQuantity <= LOW_STOCK_THRESHOLD;
                  const safePrice = Number.isFinite(product.unitPrice)
                    ? product.unitPrice
                    : 0;
                  const safeStock = Number.isFinite(product.stockQuantity)
                    ? product.stockQuantity
                    : 0;

                  return (
                    <tr key={product.productId}>
                      <td className="products-sku">{product.sku}</td>
                      <td>{product.name}</td>
                      <td>{product.category}</td>
                      <td>R{safePrice.toFixed(2)}</td>
                      <td className={isLowStock ? "products-stock-low" : ""}>
                        {safeStock}
                      </td>
                      <td>
                        <span
                          className={`products-badge ${
                            product.isActive
                              ? "products-badge-active"
                              : "products-badge-inactive"
                          }`}
                        >
                          {product.isActive ? "Active" : "Inactive"}
                        </span>
                      </td>
                    </tr>
                  );
                })
              )}
            </tbody>
          </table>
        </div>
      )}

      <p className="products-footnote">
        <span className="products-footnote-low">Red stock = low stock</span>{" "}
        (at or below {LOW_STOCK_THRESHOLD} units). View only: product changes are
        made by Manager/Admin.
      </p>
    </section>
  );
}