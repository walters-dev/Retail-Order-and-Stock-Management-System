import { useMemo, useState } from 'react';
import { useNavigate } from 'react-router';
import './Products.css';

const LOW_STOCK_THRESHOLD = 50;
const PAGE_SIZE = 10;

function normalizeProduct(raw, index = 0) {
  const item = raw ?? {};
  const rawPrice = Number(item.unitPrice ?? item.price ?? 0);
  const rawStock = Number(item.stockQuantity ?? item.stock ?? 0);

  return {
    productId: item.productId ?? item.id ?? index + 1,
    sku: String(item.sku ?? item.SKU ?? '').trim(),
    name: String(item.name ?? item.productName ?? '').trim(),
    category: String(item.category ?? 'General').trim() || 'General',
    unitPrice: Number.isFinite(rawPrice) ? rawPrice : 0,
    stockQuantity: Number.isFinite(rawStock) ? rawStock : 0,
    isActive: Boolean(item.isActive ?? item.active ?? true),
  };
}

export default function Products({
  products = [],
  loading = false,
  error = '',
  onEditProduct,
}) {
  const navigate = useNavigate();
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedCategory, setSelectedCategory] = useState('ALL');
  const [currentPage, setCurrentPage] = useState(1);

  const normalizedProducts = useMemo(() => {
    const list = Array.isArray(products) ? products : [];
    return list.map((item, idx) => normalizeProduct(item, idx));
  }, [products]);

  const categories = useMemo(() => {
    const unique = new Set(
      normalizedProducts.map((p) => p.category).filter(Boolean)
    );
    return Array.from(unique).sort((a, b) => a.localeCompare(b));
  }, [normalizedProducts]);

  const filteredProducts = useMemo(() => {
    const query = searchTerm.trim().toLowerCase();

    return normalizedProducts.filter((product) => {
      const matchesCategory =
        selectedCategory === 'ALL' || product.category === selectedCategory;

      if (!matchesCategory) return false;
      if (!query) return true;

      return (
        product.sku.toLowerCase().includes(query) ||
        product.name.toLowerCase().includes(query)
      );
    });
  }, [normalizedProducts, searchTerm, selectedCategory]);

  const totalPages = Math.max(1, Math.ceil(filteredProducts.length / PAGE_SIZE));
  const safePage = Math.min(currentPage, totalPages);

  const paginatedProducts = useMemo(() => {
    const start = (safePage - 1) * PAGE_SIZE;
    return filteredProducts.slice(start, start + PAGE_SIZE);
  }, [filteredProducts, safePage]);

  const activeCount = useMemo(
    () => normalizedProducts.filter((p) => p.isActive).length,
    [normalizedProducts]
  );

  const lowStockCount = useMemo(
    () =>
      normalizedProducts.filter(
        (p) => p.isActive && p.stockQuantity <= LOW_STOCK_THRESHOLD
      ).length,
    [normalizedProducts]
  );

  function handleSearchChange(value) {
    setSearchTerm(value);
    setCurrentPage(1);
  }

  function handleCategoryChange(value) {
    setSelectedCategory(value);
    setCurrentPage(1);
  }

  function handleNavigateToEdit(product = null) {
    onEditProduct?.(product);
    navigate('/product-management', {
      state: product ? { product } : undefined,
    });
  }

  return (
    <section className="products-page">
      <div className="products-card">
        <div className="products-header">
          <div>
            <h1 className="products-title">Products</h1>
            <p className="products-subtitle">
              Active products and current stock ({activeCount} active -{' '}
              {lowStockCount} low stock)
            </p>
          </div>

          <div className="products-header-actions">
            <button
              type="button"
              className="products-secondary-btn"
              onClick={() => handleNavigateToEdit(null)}
              disabled={loading}
            >
              Edit Product
            </button>
          </div>
        </div>

        <div className="products-toolbar">
          <input
            type="search"
            className="products-search"
            placeholder="Search by SKU or name"
            aria-label="Search by SKU or name"
            value={searchTerm}
            onChange={(e) => handleSearchChange(e.target.value)}
            disabled={loading}
          />

          <select
            className="products-category-select"
            aria-label="Filter by category"
            value={selectedCategory}
            onChange={(e) => handleCategoryChange(e.target.value)}
            disabled={loading}
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
          <p className="products-loading">Loading products...</p>
        )}

        {!loading && error && (
          <div role="alert" className="products-error">
            {error}
          </div>
        )}

        {!loading && !error && (
          <>
            <div className="products-table-wrap">
              <table className="products-table">
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
                  {paginatedProducts.length === 0 ? (
                    <tr>
                      <td colSpan={7} className="products-empty">
                        No products match your search or category filter.
                      </td>
                    </tr>
                  ) : (
                    paginatedProducts.map((product) => {
                      const isLowStock =
                        product.stockQuantity <= LOW_STOCK_THRESHOLD;

                      return (
                        <tr key={product.productId}>
                          <td className="products-sku">{product.sku}</td>
                          <td>{product.name}</td>
                          <td>{product.category}</td>
                          <td>R{product.unitPrice.toFixed(2)}</td>
                          <td
                            className={isLowStock ? 'products-stock-low' : ''}
                          >
                            {product.stockQuantity}
                          </td>
                          <td>
                            <span
                              className={`products-status-pill ${
                                product.isActive
                                  ? 'products-status-active'
                                  : 'products-status-inactive'
                              }`}
                            >
                              {product.isActive ? 'Active' : 'Inactive'}
                            </span>
                          </td>
                          <td>
                            <button
                              type="button"
                              className="products-row-edit-btn"
                              onClick={() => handleNavigateToEdit(product)}
                            >
                              Edit Product
                            </button>
                          </td>
                        </tr>
                      );
                    })
                  )}
                </tbody>
              </table>
            </div>

            {filteredProducts.length > 0 && (
              <div className="products-pagination">
                <button
                  type="button"
                  className="products-secondary-btn"
                  onClick={() => setCurrentPage((p) => Math.max(1, p - 1))}
                  disabled={safePage <= 1}
                >
                  Previous
                </button>

                <span className="products-page-info">
                  Page {safePage} of {totalPages} ({filteredProducts.length}{' '}
                  items)
                </span>

                <button
                  type="button"
                  className="products-secondary-btn"
                  onClick={() =>
                    setCurrentPage((p) => Math.min(totalPages, p + 1))
                  }
                  disabled={safePage >= totalPages}
                >
                  Next
                </button>
              </div>
            )}
          </>
        )}

        <p className="products-footnote">
          <span className="products-footnote-low">Red stock = low stock</span>{' '}
          ( &lt;= {LOW_STOCK_THRESHOLD})
        </p>
      </div>
    </section>
  );
}