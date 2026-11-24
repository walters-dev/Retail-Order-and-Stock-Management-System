import { useState } from "react";
import { createProduct, updateProduct } from "../services/api";
import "./ProductManagement.css";

const EMPTY_FORM = {
  sku: "",
  name: "",
  category: "",
  unitPrice: "",
  stockQuantity: "0",
  isActive: true,
};

function getProductId(product) {
  return product.productId ?? product.id;
}

function toForm(product) {
  return {
    sku: String(product.sku ?? ""),
    name: String(product.name ?? product.productName ?? ""),
    category: String(product.category ?? ""),
    unitPrice: String(product.unitPrice ?? product.price ?? ""),
    stockQuantity: String(product.stockQuantity ?? product.stock ?? 0),
    isActive: Boolean(product.isActive ?? product.active ?? true),
  };
}

export default function ProductManagement({ products = [], onSaved }) {
  const [form, setForm] = useState(EMPTY_FORM);
  const [editingId, setEditingId] = useState(null);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [successMessage, setSuccessMessage] = useState("");

  const isEditMode = editingId !== null;

  function handleChange(field, value) {
    setForm((current) => ({ ...current, [field]: value }));
    setError("");
    setSuccessMessage("");
  }

  function handleNewProduct() {
    setEditingId(null);
    setForm(EMPTY_FORM);
    setError("");
    setSuccessMessage("");
  }

  function handleEditProduct(product) {
    setEditingId(getProductId(product));
    setForm(toForm(product));
    setError("");
    setSuccessMessage("");
  }

  function validateForm() {
    const sku = form.sku.trim();
    const name = form.name.trim();
    const unitPrice = Number(form.unitPrice);
    const stockQuantity = Number(form.stockQuantity);

    if (!sku) return "SKU is required.";
    if (!name) return "Product name is required.";
    if (!Number.isFinite(unitPrice) || unitPrice < 0.01) {
      return "Unit price must be at least R0.01.";
    }
    if (!Number.isInteger(stockQuantity) || stockQuantity < 0) {
      return "Stock quantity must be a whole number of zero or more.";
    }

    const duplicateSku = products.some(
      (product) =>
        getProductId(product) !== editingId &&
        String(product.sku ?? "").trim().toLowerCase() === sku.toLowerCase()
    );
    if (duplicateSku) return `SKU "${sku}" is already in use.`;

    return "";
  }

  async function handleSubmit(event) {
    event.preventDefault();

    const validationError = validateForm();
    if (validationError) {
      setError(validationError);
      return;
    }

    const payload = {
      sku: form.sku.trim(),
      name: form.name.trim(),
      category: form.category.trim() || "General",
      unitPrice: Number(Number(form.unitPrice).toFixed(2)),
      stockQuantity: Number(form.stockQuantity),
      isActive: form.isActive,
    };

    setSaving(true);
    setError("");
    setSuccessMessage("");

    try {
      if (isEditMode) {
        await updateProduct(editingId, payload);
        setSuccessMessage(`Updated ${payload.sku} (${payload.name}).`);
      } else {
        await createProduct(payload);
        setSuccessMessage(`Created ${payload.sku} (${payload.name}).`);
        setForm(EMPTY_FORM);
      }

      await onSaved?.();
    } catch (saveError) {
      setError(saveError.message || "Failed to save product.");
    } finally {
      setSaving(false);
    }
  }

  return (
    <section className="product-management" aria-label="Product management">
      <header className="product-management-header">
        <div>
          <h1 className="product-management-title">Product Management</h1>
          <p className="product-management-subtitle">
            Add products or update existing product details and stock.
          </p>
        </div>
        <button
          type="button"
          className="product-management-secondary-button"
          onClick={handleNewProduct}
          disabled={saving}
        >
          + New product
        </button>
      </header>

      {error && (
        <div className="product-management-message product-management-error" role="alert">
          {error}
        </div>
      )}
      {successMessage && (
        <div className="product-management-message product-management-success" role="status">
          {successMessage}
        </div>
      )}

      <form className="product-management-form" onSubmit={handleSubmit}>
        <div className="product-management-form-header">
          <h2>{isEditMode ? "Edit product" : "Add a product"}</h2>
          {isEditMode && <span>Editing product ID {editingId}</span>}
        </div>

        <div className="product-management-fields">
          <div className="product-management-field">
            <label htmlFor="management-sku">SKU</label>
            <input
              id="management-sku"
              name="sku"
              autoComplete="off"
              value={form.sku}
              onChange={(event) => handleChange("sku", event.target.value)}
              required
            />
          </div>
          <div className="product-management-field">
            <label htmlFor="management-name">Product name</label>
            <input
              id="management-name"
              name="name"
              value={form.name}
              onChange={(event) => handleChange("name", event.target.value)}
              required
            />
          </div>
          <div className="product-management-field">
            <label htmlFor="management-category">Category</label>
            <input
              id="management-category"
              name="category"
              value={form.category}
              onChange={(event) => handleChange("category", event.target.value)}
            />
          </div>
          <div className="product-management-field">
            <label htmlFor="management-price">Unit price (R)</label>
            <input
              id="management-price"
              name="unitPrice"
              type="number"
              min="0.01"
              step="0.01"
              value={form.unitPrice}
              onChange={(event) => handleChange("unitPrice", event.target.value)}
              required
            />
          </div>
          <div className="product-management-field">
            <label htmlFor="management-stock">Stock quantity</label>
            <input
              id="management-stock"
              name="stockQuantity"
              type="number"
              min="0"
              step="1"
              value={form.stockQuantity}
              onChange={(event) => handleChange("stockQuantity", event.target.value)}
              required
            />
          </div>
          <label className="product-management-active-field" htmlFor="management-active">
            <input
              id="management-active"
              name="isActive"
              type="checkbox"
              checked={form.isActive}
              onChange={(event) => handleChange("isActive", event.target.checked)}
            />
            Active product
          </label>
        </div>

        <div className="product-management-form-actions">
          {isEditMode && (
            <button
              type="button"
              className="product-management-secondary-button"
              onClick={handleNewProduct}
              disabled={saving}
            >
              Cancel edit
            </button>
          )}
          <button type="submit" className="product-management-primary-button" disabled={saving}>
            {saving ? "Saving..." : isEditMode ? "Save changes" : "Add product"}
          </button>
        </div>
      </form>

      <section className="product-management-inventory" aria-labelledby="inventory-title">
        <div className="product-management-inventory-header">
          <div>
            <h2 id="inventory-title">Inventory</h2>
            <p>{products.length} products</p>
          </div>
        </div>
        <div className="product-management-table-wrapper">
          <table className="product-management-table">
            <thead>
              <tr>
                <th scope="col">SKU</th>
                <th scope="col">Product</th>
                <th scope="col">Category</th>
                <th scope="col">Unit price</th>
                <th scope="col">Stock</th>
                <th scope="col">Status</th>
                <th scope="col"><span className="visually-hidden">Actions</span></th>
              </tr>
            </thead>
            <tbody>
              {products.length === 0 ? (
                <tr>
                  <td colSpan={7} className="product-management-empty">
                    No products available.
                  </td>
                </tr>
              ) : (
                products.map((product, index) => {
                  const productId = getProductId(product);
                  const isActive = Boolean(product.isActive ?? product.active ?? true);
                  const price = Number(product.unitPrice ?? product.price ?? 0);
                  const stock = Number(product.stockQuantity ?? product.stock ?? 0);

                  return (
                    <tr key={productId ?? product.sku ?? index}>
                      <td className="product-management-sku">{product.sku}</td>
                      <td>{product.name ?? product.productName}</td>
                      <td>{product.category || "General"}</td>
                      <td>R{Number.isFinite(price) ? price.toFixed(2) : "0.00"}</td>
                      <td>{Number.isFinite(stock) ? stock : 0}</td>
                      <td>
                        <span
                          className={`product-management-status ${
                            isActive ? "is-active" : "is-inactive"
                          }`}
                        >
                          {isActive ? "Active" : "Inactive"}
                        </span>
                      </td>
                      <td>
                        <button
                          type="button"
                          className="product-management-edit-button"
                          onClick={() => handleEditProduct(product)}
                          disabled={saving}
                          aria-label={`Edit ${product.name ?? product.productName}`}
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
      </section>
    </section>
  );
}