import { useMemo, useState } from "react";

const LOW_STOCK_THRESHOLD = 5;

function normalizeProduct(raw, index) {
  return {
    productId: raw.productId ?? raw.id ?? index + 1,
    sku: raw.sku ?? raw.SKU ?? "",
    name: raw.name ?? raw.productName ?? "",
    category: raw.category ?? "General",
    unitPrice: Number(raw.unitPrice ?? raw.price ?? 0),
    stockQuantity: Number(raw.stockQuantity ?? raw.stock ?? 0),
    isActive: Boolean(raw.isActive ?? raw.active ?? true),
  };
}

export default function Products({
  products = [],
  loading = false,
  error = "",
  onRefresh,
  onEditProduct,
  onCreateOrder,
}) {
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedCategory, setSelectedCategory] = useState("ALL");

  const normalizedProducts = useMemo(
    () => products.map((item, idx) => normalizeProduct(item, idx)),
    [products]
  );

  const categories = useMemo(() => {
    const unique = new Set(
      normalizedProducts.map((p) => p.category).filter(Boolean)
    );
    return [...unique].sort((a, b) => a.localeCompare(b));
  }, [normalizedProducts]);

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

  const activeCount = normalizedProducts.filter((p) => p.isActive).length;
  const lowStockCount = normalizedProducts.filter(
    (p) => p.isActive && p.stockQuantity <= LOW_STOCK_THRESHOLD
  ).length;

  return (
    <section className="panel">
      <div className="panel-top">
        <div>
          <h2>Products</h2>
          <p className="subtitle">
            Active products and current stock ({activeCount} active • {lowStockCount} low stock).[cite: 1, 2]
          </p>
        </div>

        <div className="action-row">
          {onRefresh && (
            <button
              type="button"
              className="btn-secondary"
              onClick={onRefresh}
              disabled={loading}
            >
              {loading ? "Refreshing..." : "Refresh"}
            </button>
          )}
          {onCreateOrder && (
            <button type="button" className="btn-primary" onClick={onCreateOrder}>
              + Create Order
            </button>
          )}
        </div>
      </div>

      <div className="toolbar">
        <input
          type="search"
          className="search-input"
          placeholder="Search by SKU or name"
          aria-label="Search by SKU or name"
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
        />

        <select
          className="select-input"
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

      {loading && <p className="muted">Loading products...</p>}

      {!loading && error && (
        <div className="error-banner" role="alert">
          {error}
        </div>
      )}

      {!loading && !error && (
        <div className="table-card">
          <table>
            <thead>
              <tr>
                <th>SKU</th>
                <th>Product</th>
                <th>Category</th>
                <th>Price</th>
                <th>Stock</th>
                <th>Status</th>
                {onEditProduct && <th>Action</th>}
              </tr>
            </thead>
            <tbody>
              {filteredProducts.length === 0 ? (
                <tr>
                  <td colSpan={onEditProduct ? 7 : 6} className="muted center-cell">
                    No products match your search or category filter.
                  </td>
                </tr>
              ) : (
                filteredProducts.map((product) => {
                  const isLowStock = product.stockQuantity <= LOW_STOCK_THRESHOLD;

                  return (
                    <tr key={product.productId}>
                      <td className="mono">{product.sku}</td>
                      <td>{product.name}</td>
                      <td>{product.category}</td>
                      <td>R{product.unitPrice.toFixed(2)}</td>
                      <td className={isLowStock ? "low-stock-text" : ""}>
                        {product.stockQuantity}
                      </td>
                      <td>
                        <span
                          className={`status-pill ${
                            product.isActive ? "status-active" : "status-inactive"
                          }`}
                        >
                          {product.isActive ? "Active" : "Inactive"}
                        </span>
                      </td>
                      {onEditProduct && (
                        <td>
                          <button
                            type="button"
                            className="btn-link"
                            onClick={() => onEditProduct(product)}
                          >
                            Edit
                          </button>
                        </td>
                      )}
                    </tr>
                  );
                })
              )}
            </tbody>
          </table>
        </div>
      )}

      <p className="footnote">
        <span className="low-stock-text">Red stock = low stock</span> (≤ {LOW_STOCK_THRESHOLD} units).
        Inactive products remain stored for historical reference but cannot be added to new quotes or orders.[cite: 1, 2]
      </p>
    </section>
  );
}