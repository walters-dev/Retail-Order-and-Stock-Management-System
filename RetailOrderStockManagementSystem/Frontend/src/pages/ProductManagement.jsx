import { useMemo, useState } from 'react';
import './ProductManagement.css';
import { useAuth } from '../auth/authContext.jsx';

const INITIAL_FORM = {
  sku: '',
  category: '',
  name: '',
  unitPrice: '',
  stockQuantity: '',
  isActive: true,
};

function normalizeProduct(raw, index = 0) {
  const item = raw ?? {};
  const rawPrice = Number(item.unitPrice ?? item.price ?? 0);
  const rawStock = Number(item.stockQuantity ?? item.stock ?? 0);

  return {
    productId: item.productId ?? item.id ?? index + 1,
    sku: String(item.sku ?? item.SKU ?? '').trim(),
    name: String(item.name ?? item.productName ?? '').trim(),
    category: String(item.category ?? '').trim(),
    unitPrice: Number.isFinite(rawPrice) ? rawPrice : 0,
    stockQuantity: Number.isFinite(rawStock) ? rawStock : 0,
    isActive: Boolean(item.isActive ?? item.active ?? true),
  };
}

export default function ProductManagement({
  products = [],
  categories = [],
  selectedProduct = null,
  onSelectProduct,
  onSaved,
  onCancel,
  onRefresh,
  loading = false,
}) {
  const { token } = useAuth();

  const normalizedProducts = useMemo(() => {
    const list = Array.isArray(products) ? products : [];
    return list.map((item, idx) => normalizeProduct(item, idx));
  }, [products]);

  const availableCategories = useMemo(() => {
    const set = new Set(
      [
        ...(Array.isArray(categories) ? categories : []),
        ...normalizedProducts.map((p) => p.category),
      ]
        .map((c) => String(c ?? '').trim())
        .filter((c) => c && c !== 'ALL')
    );
    return Array.from(set).sort((a, b) => a.localeCompare(b));
  }, [categories, normalizedProducts]);

  const initialProduct = selectedProduct
    ? normalizeProduct(selectedProduct)
    : null;
  const isEditing = Boolean(initialProduct);

  const [lookupQuery, setLookupQuery] = useState('');
  const [form, setForm] = useState(() =>
    initialProduct
      ? {
          sku: initialProduct.sku,
          category: initialProduct.category,
          name: initialProduct.name,
          unitPrice: String(initialProduct.unitPrice),
          stockQuantity: String(initialProduct.stockQuantity),
          isActive: initialProduct.isActive,
        }
      : INITIAL_FORM
  );
  const [error, setError] = useState('');
  const [statusMessage, setStatusMessage] = useState('');
  const [pendingPayload, setPendingPayload] = useState(null);
  const [isSaving, setIsSaving] = useState(false);

  const matchingProducts = useMemo(() => {
    const q = lookupQuery.trim().toLowerCase();
    if (!q) return [];
    return normalizedProducts
      .filter(
        (p) => p.sku.toLowerCase().includes(q) || p.name.toLowerCase().includes(q)
      )
      .slice(0, 6);
  }, [lookupQuery, normalizedProducts]);

  function applyProductToForm(product) {
    const norm = normalizeProduct(product);
    setForm({
      sku: norm.sku,
      category: norm.category,
      name: norm.name,
      unitPrice: String(norm.unitPrice),
      stockQuantity: String(norm.stockQuantity),
      isActive: norm.isActive,
    });
    setError('');
    setStatusMessage('');
    setPendingPayload(null);
    onSelectProduct?.(norm);
  }

  function handleLookupChange(value) {
    setLookupQuery(value);
    setError('');
    setStatusMessage('');
    setPendingPayload(null);

    const trimmed = value.trim().toLowerCase();
    if (!trimmed) return;

    const exactMatch = normalizedProducts.find(
      (p) => p.sku.toLowerCase() === trimmed || p.name.toLowerCase() === trimmed
    );
    if (exactMatch) {
      applyProductToForm(exactMatch);
    }
  }

  function handleResetNew() {
    setLookupQuery('');
    setForm(INITIAL_FORM);
    setError('');
    setStatusMessage('');
    setPendingPayload(null);
    onSelectProduct?.(null);
  }

  function handleFormSubmit(event) {
    event.preventDefault();
    setError('');
    setStatusMessage('');

    const sku = form.sku.trim();
    const name = form.name.trim();
    const category = form.category.trim();
    const unitPrice = Number(form.unitPrice);
    const stockQuantity = Number(form.stockQuantity);

    if (!isEditing && !sku) {
      setError('Enter a SKU or select an existing product to edit.');
      return;
    }

    if (!name) {
      setError('Enter a product name.');
      return;
    }

    if (!Number.isFinite(unitPrice) || unitPrice <= 0) {
      setError('Unit price must be greater than R0.');
      return;
    }

    if (!Number.isInteger(stockQuantity) || stockQuantity < 0) {
      setError('Stock quantity cannot be negative.');
      return;
    }

    const productId = initialProduct?.productId ?? null;
    setPendingPayload({
      isEditing,
      productId,
      sku,
      name,
      category,
      unitPrice,
      stockQuantity,
      isActive: form.isActive,
    });
  }

  async function handleConfirmSave() {
    if (!pendingPayload) return;
    setError('');
    setStatusMessage('');

    const url = pendingPayload.isEditing
      ? `/api/products/${pendingPayload.productId}`
      : '/api/products';
    const headers = { 'Content-Type': 'application/json' };
    if (token) {
      headers.Authorization = `Bearer ${token}`;
    }

    setIsSaving(true);
    try {
      const response = await fetch(url, {
        method: pendingPayload.isEditing ? 'PUT' : 'POST',
        headers,
        body: JSON.stringify({
          ...(pendingPayload.isEditing
            ? { productId: pendingPayload.productId }
            : {}),
          sku: pendingPayload.sku,
          name: pendingPayload.name,
          category: pendingPayload.category,
          unitPrice: pendingPayload.unitPrice,
          stockQuantity: pendingPayload.stockQuantity,
          isActive: pendingPayload.isActive,
        }),
      });

      if (!response.ok) {
        const problem = await response.json().catch(() => null);
        setError(problem?.message || 'Product could not be saved.');
        return;
      }

      const wasEditing = pendingPayload.isEditing;
      setPendingPayload(null);
      setStatusMessage(
        wasEditing
          ? `Updated "${pendingPayload.name}" (${pendingPayload.sku}).`
          : `Saved "${pendingPayload.name}" (${pendingPayload.sku}).`
      );
      if (!wasEditing) {
        setForm(INITIAL_FORM);
        setLookupQuery('');
      }
      onSaved?.();
    } catch (err) {
      console.error(err);
      setError('Unable to save the product. Please try again.');
    } finally {
      setIsSaving(false);
    }
  }

  return (
    <section className="product-management">
      <div className="product-management-card">
        <div className="product-management-header">
          <div>
            <h1 className="product-management-title">Product Management</h1>
            <p className="product-management-subtitle">
              Create and update products in the catalog
            </p>
          </div>

          <div className="product-management-header-actions">
            {isEditing && (
              <button
                type="button"
                className="product-management-secondary-button"
                onClick={handleResetNew}
                disabled={isSaving || loading}
              >
                + New Product
              </button>
            )}
            {onRefresh && (
              <button
                type="button"
                className="product-management-primary-button"
                onClick={onRefresh}
                disabled={isSaving || loading}
              >
                {loading ? 'Refreshing...' : 'Refresh'}
              </button>
            )}
          </div>
        </div>

        <div className="product-management-lookup">
          <label htmlFor="prod-lookup">Find product to edit (SKU or name)</label>
          <input
            id="prod-lookup"
            type="search"
            placeholder="Search by SKU or product name to auto-fill..."
            value={lookupQuery}
            onChange={(e) => handleLookupChange(e.target.value)}
            disabled={isSaving}
          />
          {matchingProducts.length > 0 && (
            <ul className="product-management-lookup-results" role="listbox">
              {matchingProducts.map((item) => (
                <li key={item.productId}>
                  <button
                    type="button"
                    className="product-management-lookup-item"
                    onClick={() => {
                      setLookupQuery(`${item.sku} - ${item.name}`);
                      applyProductToForm(item);
                    }}
                    disabled={isSaving}
                  >
                    <span className="product-management-sku">{item.sku}</span>
                    <span>{item.name}</span>
                    <span className="product-management-lookup-meta">
                      {item.category || 'Uncategorized'} • R
                      {item.unitPrice.toFixed(2)} • Stock: {item.stockQuantity}
                    </span>
                  </button>
                </li>
              ))}
            </ul>
          )}
        </div>

        {error && (
          <p
            role="alert"
            className="product-management-message product-management-error"
          >
            {error}
          </p>
        )}

        {statusMessage && (
          <p
            role="status"
            className="product-management-message product-management-success"
          >
            {statusMessage}
          </p>
        )}

        <form onSubmit={handleFormSubmit} className="product-management-form">
          <div className="product-management-fields">
            <div className="product-management-field">
              <label htmlFor="prod-sku">
                SKU {isEditing ? '(auto-generated, locked)' : '*'}
              </label>
              <input
                id="prod-sku"
                type="text"
                value={form.sku}
                readOnly={isEditing}
                disabled={isSaving || isEditing}
                placeholder={isEditing ? 'Auto-generated SKU' : 'e.g. SKU-004'}
                onChange={(e) => {
                  setForm((prev) => ({ ...prev, sku: e.target.value }));
                  setError('');
                  setPendingPayload(null);
                }}
              />
            </div>

            <div className="product-management-field">
              <label htmlFor="prod-category">Category</label>
              <select
                id="prod-category"
                value={form.category}
                disabled={isSaving}
                onChange={(e) => {
                  setForm((prev) => ({ ...prev, category: e.target.value }));
                  setError('');
                  setPendingPayload(null);
                }}
              >
                <option value="">All categories</option>
                {availableCategories.map((cat) => (
                  <option key={cat} value={cat}>
                    {cat}
                  </option>
                ))}
              </select>
            </div>

            <div className="product-management-field product-management-field-full">
              <label htmlFor="prod-name">Product name *</label>
              <input
                id="prod-name"
                type="text"
                value={form.name}
                disabled={isSaving}
                onChange={(e) => {
                  setForm((prev) => ({ ...prev, name: e.target.value }));
                  setError('');
                  setPendingPayload(null);
                }}
              />
            </div>

            <div className="product-management-field">
              <label htmlFor="prod-price">Unit price (R) *</label>
              <input
                id="prod-price"
                type="number"
                step="0.01"
                min="0.01"
                value={form.unitPrice}
                disabled={isSaving}
                onChange={(e) => {
                  setForm((prev) => ({ ...prev, unitPrice: e.target.value }));
                  setError('');
                  setPendingPayload(null);
                }}
              />
            </div>

            <div className="product-management-field">
              <label htmlFor="prod-stock">Stock quantity *</label>
              <input
                id="prod-stock"
                type="number"
                step="1"
                min="0"
                value={form.stockQuantity}
                disabled={isSaving}
                onChange={(e) => {
                  setForm((prev) => ({
                    ...prev,
                    stockQuantity: e.target.value,
                  }));
                  setError('');
                  setPendingPayload(null);
                }}
              />
            </div>
          </div>

          <div className="product-management-active-row">
            <input
              id="prod-active"
              type="checkbox"
              checked={form.isActive}
              disabled={isSaving}
              onChange={(e) => {
                setForm((prev) => ({ ...prev, isActive: e.target.checked }));
                setError('');
                setPendingPayload(null);
              }}
            />
            <label htmlFor="prod-active">Active</label>
          </div>

          {pendingPayload && (
            <div className="product-management-confirm-box" role="region" aria-label="Confirm product changes">
              <p className="product-management-confirm-text">
                Are you sure the input you just applied is what you want before
                saving? ({pendingPayload.name} • Category:{' '}
                {pendingPayload.category || 'None'} • Price: R
                {pendingPayload.unitPrice.toFixed(2)} • Stock:{' '}
                {pendingPayload.stockQuantity} •{' '}
                {pendingPayload.isActive ? 'Active' : 'Inactive'})
              </p>
              <div className="product-management-confirm-actions">
                <button
                  type="button"
                  className="product-management-secondary-button"
                  onClick={() => setPendingPayload(null)}
                  disabled={isSaving}
                >
                  Review Edits
                </button>
                <button
                  type="button"
                  className="product-management-primary-button"
                  onClick={handleConfirmSave}
                  disabled={isSaving}
                >
                  {isSaving ? 'Saving...' : 'Confirm & Save'}
                </button>
              </div>
            </div>
          )}

          <div className="product-management-footer-bar">
            <p className="product-management-hint">
              SKU must be unique and cannot be edited once created. Price must
              be above R0. Stock cannot be negative.
            </p>

            <div className="product-management-actions">
              {onCancel && (
                <button
                  type="button"
                  className="product-management-secondary-button"
                  onClick={() => {
                    setPendingPayload(null);
                    onCancel();
                  }}
                  disabled={isSaving}
                >
                  Cancel
                </button>
              )}
              <button
                type="submit"
                className="product-management-primary-button"
                disabled={isSaving}
              >
                {isSaving
                  ? 'Saving...'
                  : isEditing
                  ? 'Update Product'
                  : 'Save Product'}
              </button>
            </div>
          </div>
        </form>
      </div>
    </section>
  );
}