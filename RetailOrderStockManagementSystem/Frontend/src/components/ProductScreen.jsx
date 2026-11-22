import { useMemo, useState } from "react";

const LOW_STOCK_THRESHOLD = 5;

export default function ProductsScreen({
  products,
  loading,
  error,
  onSelectEditProduct,
  onGoToCreateOrder,
}) {
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedCategory, setSelectedCategory] = useState("ALL");

  const categories = useMemo(() => {
    const unique = [...new Set(products.map((p) => p.category).filter(Boolean))];
    return unique.sort();
  }, [products]);

  const filteredProducts = useMemo(() => {
    const q = searchTerm.trim().toLowerCase();

    return products.filter((item) => {
      const matchesCategory =
        selectedCategory === "ALL" || item.category === selectedCategory;

      const matchesSearch =
        !q ||
        item.sku.toLowerCase().includes(q) ||
        item.name.toLowerCase().includes(q);

      return matchesCategory && matchesSearch;
    });
  }, [products, searchTerm, selectedCategory]);

  if (loading) {
    return <section className="panel"><p className="muted">Loading products and stock...</p></section>;
  }

  if (error) {
    return (
      <section className="panel">
        <p className="error-banner">{error}</p>
      </section>
    );
  }

  return (
    <section className="panel">
      <div className="panel-top">
        <div>
          <h2>Products</h2>
          <p className="subtitle">Active products and current stock.</p>
        </div>
        <button type="button" className="btn-primary" onClick={onGoToCreateOrder}>
          + Create Order
        </button>
      </div>

      <div className="toolbar">
        <input
          type="search"
          className="search-input"
          placeholder="Search by SKU or name"
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
        />

        <select
          className="select-input"
          value={selectedCategory}
          onChange={(e) => setSelectedCategory(e.target.value)}
        >
          <option value="ALL">All categories</option>
          {categories.map((cat) => (
            <option key={cat} value={cat}>
              {cat}
            </option>
          ))}
        </select>
      </div>

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
              <th>Action</th>
            </tr>
          </thead>
          <tbody>
            {filteredProducts.length === 0 ? (
              <tr>
                <td colSpan="7" className="muted center-cell">
                  No products match your filter.
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
                    <td>R{Number(product.unitPrice).toFixed(2)}</td>
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
                    <td>
                      <button
                        type="button"
                        className="btn-link"
                        onClick={() => onSelectEditProduct(product)}
                      >
                        Edit
                      </button>
                    </td>
                  </tr>
                );
              })
            )}
          </tbody>
        </table>
      </div>

      <p className="footnote">
        <span className="low-stock-text">Red stock = low stock</span> (at or below{" "}
        {LOW_STOCK_THRESHOLD} units). Inactive products remain stored but cannot be added to new
        quotes or orders[cite: 2].
      </p>
    </section>
  );
}