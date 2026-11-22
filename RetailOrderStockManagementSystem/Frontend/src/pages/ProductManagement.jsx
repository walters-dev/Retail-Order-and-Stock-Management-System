import { useEffect, useState } from "react";
import { createProduct, updateProduct } from "../services/api";

const EMPTY_FORM = {
  sku: "",
  category: "Accessories",
  name: "",
  unitPrice: "",
  stockQuantity: "0",
  isActive: true,
};

export default function ProductManagement({
  products = [],
  selectedProduct = null,
  onClearSelectedProduct,
  onSaved,
  onCancel,
}) {
  const [form, setForm] = useState(EMPTY_FORM);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [successMessage, setSuccessMessage] = useState("");

  useEffect(() => {
    setError("");
    setSuccessMessage("");

    if (selectedProduct) {
      setForm({
        sku: selectedProduct.sku ?? "",
        category: selectedProduct.category ?? "General",
        name: selectedProduct.name ?? "",
        unitPrice: String(selectedProduct.unitPrice ?? selectedProduct.price ?? ""),
        stockQuantity: String(
          selectedProduct.stockQuantity ?? selectedProduct.stock ?? 0
        ),
        isActive: Boolean(selectedProduct.isActive ?? true),
      });
    } else {
      setForm(EMPTY_FORM);
    }
  }, [selectedProduct]);

  function handleChange(field, value) {
    setForm((prev) => ({ ...prev, [field]: value }));
    setError("");
    setSuccessMessage("");
  }

  function handleNewMode() {
    setError("");
    setSuccessMessage("");
    setForm(EMPTY_FORM);
    if (onClearSelectedProduct) onClearSelectedProduct();
  }

  function validateForm() {
    const trimmedSku = form.sku.trim();
    const trimmedName = form.name.trim();
    const price = Number(form.unitPrice);
    const stock = Number(form.stockQuantity);

    if (!trimmedSku) return "SKU is required.";
    if (!/^[A-Za-z]{3}[0-9]{3}[A-Za-z]{2}$/.test(trimmedSku)) {
      return "SKU must contain 3 letters, 3 digits, and 2 letters (e.g. ABC123XY).";
    }
    if (!trimmedName) return "Product name is required.";
    if (!Number.isFinite(price) || price <= 0) {
      return "Unit price must be greater than R0.00.";
    }
    if (!Number.isInteger(stock) || stock < 0) {
      return "Stock quantity cannot be negative and must be a whole number.";
    }

    const currentId = selectedProduct?.productId ?? selectedProduct?.id ?? null;
    const duplicateSku = products.some(
      (p) =>
        (p.productId ?? p.id) !== currentId &&
        (p.sku ?? "").trim().toLowerCase() === trimmedSku.toLowerCase()
    );
    if (duplicateSku) {
      return `SKU "${trimmedSku}" is already in use. SKU must be unique.`;
    }

    return "";
  }

  async function handleSubmit(e) {
    e.preventDefault();
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
      isActive: Boolean(form.isActive),
    };

    setSaving(true);
    setError("");
    setSuccessMessage("");

    try {
      const editId = selectedProduct?.productId ?? selectedProduct?.id;
      if (editId) {
        await updateProduct(editId, payload);
        setSuccessMessage(`Updated ${payload.sku} (${payload.name}).`);
      } else {
        await createProduct(payload);
        setSuccessMessage(`Created ${payload.sku} (${payload.name}).`);
        setForm(EMPTY_FORM);
      }

      if (onSaved) {
        await onSaved();
      }
    } catch (err) {
      setError(err.message || "Failed to save product.");
    } finally {
      setSaving(false);
    }
  }

  const isEditMode = Boolean(selectedProduct);

  return (
    <section className="panel">
      <div className="panel-top">
        <div>
          <h2>Product Management</h2>
          <p className="subtitle">Add or update a product.</p>
        </div>

        <div className="action-row">
          <button
            type="button"
            className="btn-secondary"
            onClick={handleNewMode}
            disabled={saving}
          >
            + New Product
          </button>
          {isEditMode && (
            <span className="editing-badge">
              Editing: {selectedProduct.sku} {selectedProduct.name}
            </span>
          )}
        </div>
      </div>

      {error && (
        <div className="error-banner" role="alert" style={{ marginBottom: "1rem" }}>
          {error}
        </div>
      )}

      {successMessage && (
        <div className="success-banner" role="status" style={{ marginBottom: "1rem" }}>
          {successMessage}
        </div>
      )}

      <form onSubmit={handleSubmit} className="product-form">
        <div className="form-grid-2">
          <div className="form-group">
            <label htmlFor="prod-sku">SKU *</label>
            <input
              id="prod-sku"
              type="text"
              placeholder="e.g. ABC123XY"
              value={form.sku}
              onChange={(e) => handleChange("sku", e.target.value)}
            />
          </div>

          <div className="form-group">
            <label htmlFor="prod-category">Category</label>
            <input
              id="prod-category"
              type="text"
              placeholder="e.g. Accessories"
              value={form.category}
              onChange={(e) => handleChange("category", e.target.value)}
            />
          </div>
        </div>

        <div className="form-group">
          <label htmlFor="prod-name">Product name *</label>
          <input
            id="prod-name"
            type="text"
            placeholder="e.g. Mechanical Keyboard"
            value={form.name}
            onChange={(e) => handleChange("name", e.target.value)}
          />
        </div>

        <div className="form-grid-2">
          <div className="form-group">
            <label htmlFor="prod-price">Unit price (R) *</label>
            <input
              id="prod-price"
              type="number"
              step="0.01"
              min="0.01"
              placeholder="0.00"
              value={form.unitPrice}
              onChange={(e) => handleChange("unitPrice", e.target.value)}
            />
          </div>

          <div className="form-group">
            <label htmlFor="prod-stock">Stock quantity *</label>
            <input
              id="prod-stock"
              type="number"
              step="1"
              min="0"
              value={form.stockQuantity}
              onChange={(e) => handleChange("stockQuantity", e.target.value)}
            />
          </div>
        </div>

        <div className="toggle-row">
          <label className="checkbox-inline" htmlFor="prod-active">
            <input
              id="prod-active"
              type="checkbox"
              checked={form.isActive}
              onChange={(e) => handleChange("isActive", e.target.checked)}
            />
            <span>Active</span>
          </label>
          <span className="muted">
            Inactive products stay stored but can&apos;t be ordered.
          </span>
        </div>

        <p className="footnote" style={{ marginTop: 0 }}>
          SKU must be unique. Price must be above R0. Stock cannot be negative.
        </p>

        <div className="form-actions-right">
          {onCancel && (
            <button
              type="button"
              className="btn-secondary"
              onClick={onCancel}
              disabled={saving}
            >
              Cancel
            </button>
          )}
          <button type="submit" className="btn-primary" disabled={saving}>
            {saving ? "Saving..." : isEditMode ? "Update Product" : "Save Product"}
          </button>
        </div>
      </form>
    </section>
  );
}