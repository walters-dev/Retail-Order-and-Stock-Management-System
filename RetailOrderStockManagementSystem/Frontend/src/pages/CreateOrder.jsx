import { useEffect, useMemo, useState } from "react";
import { getCustomers, requestOrderQuote } from "../services/api";

function makeLineId() {
  return `${Date.now()}-${Math.random().toString(36).slice(2, 7)}`;
}

export default function CreateOrder({
  products = [],
  draftOrder = null,
  onQuoteReady,
  onCancel,
}) {
  const [customers, setCustomers] = useState([]);
  const [loadingCustomers, setLoadingCustomers] = useState(true);
  const [customerId, setCustomerId] = useState(draftOrder?.customerId ?? "");
  const [rows, setRows] = useState(() => {
    if (draftOrder?.items?.length) {
      return draftOrder.items.map((item) => ({
        rowId: makeLineId(),
        productId: String(item.productId),
        quantity: String(item.quantity),
      }));
    }
    return [{ rowId: makeLineId(), productId: "", quantity: "1" }];
  });
  const [submittingQuote, setSubmittingQuote] = useState(false);
  const [error, setError] = useState("");

  const orderableProducts = useMemo(
    () =>
      products.filter(
        (p) => Boolean(p.isActive ?? p.active ?? true) && Number(p.stockQuantity ?? p.stock ?? 0) > 0
      ),
    [products]
  );

  useEffect(() => {
    let active = true;
    async function load() {
      setLoadingCustomers(true);
      try {
        const list = await getCustomers();
        if (!active) return;
        setCustomers(list);
        if (!draftOrder?.customerId && list.length > 0) {
          setCustomerId(String(list[0].customerId ?? list[0].id));
        }
      } catch (err) {
        if (active) setError(err.message || "Failed to load customers.");
      } finally {
        if (active) setLoadingCustomers(false);
      }
    }
    load();
    return () => {
      active = false;
    };
  }, [draftOrder?.customerId]);

  useEffect(() => {
    if (orderableProducts.length === 0) return;
    setRows((prev) =>
      prev.map((row) => {
        if (row.productId) return row;
        return { ...row, productId: String(orderableProducts[0].productId ?? orderableProducts[0].id) };
      })
    );
  }, [orderableProducts]);

  function getProductById(idValue) {
    return orderableProducts.find(
      (p) => String(p.productId ?? p.id) === String(idValue)
    );
  }

  function handleAddRow() {
    const usedIds = new Set(rows.map((r) => r.productId));
    const nextAvailable = orderableProducts.find(
      (p) => !usedIds.has(String(p.productId ?? p.id))
    );
    setError("");
    setRows((prev) => [
      ...prev,
      {
        rowId: makeLineId(),
        productId: nextAvailable
          ? String(nextAvailable.productId ?? nextAvailable.id)
          : String(orderableProducts[0]?.productId ?? orderableProducts[0]?.id ?? ""),
        quantity: "1",
      },
    ]);
  }

  function handleRemoveRow(rowId) {
    setError("");
    setRows((prev) => prev.filter((r) => r.rowId !== rowId));
  }

  function handleRowChange(rowId, field, value) {
    setError("");
    setRows((prev) =>
      prev.map((row) => (row.rowId === rowId ? { ...row, [field]: value } : row))
    );
  }

  async function handleRequestQuote(e) {
    e.preventDefault();
    setError("");

    if (!customerId) {
      setError("Please select a customer.");
      return;
    }

    if (rows.length === 0) {
      setError("Please add at least one product to the order.");
      return;
    }

    const seenProductIds = new Set();
    const items = [];

    for (const row of rows) {
      const product = getProductById(row.productId);
      const qty = Number(row.quantity);

      if (!product) {
        setError("Please select a valid active product for every row.");
        return;
      }

      const prodId = product.productId ?? product.id;
      if (seenProductIds.has(prodId)) {
        setError(`Duplicate product "${product.name}". Combine quantities into a single row.`);
        return;
      }
      seenProductIds.add(prodId);

      if (!Number.isInteger(qty) || qty < 1) {
        setError(`Quantity for "${product.name}" must be a positive whole number.`);
        return;
      }

      const maxStock = Number(product.stockQuantity ?? product.stock ?? 0);
      if (qty > maxStock) {
        setError(
          `Quantity for "${product.name}" cannot exceed available stock (${maxStock}).`
        );
        return;
      }

      items.push({
        productId: prodId,
        quantity: qty,
      });
    }

    setSubmittingQuote(true);
    try {
      const selectedCustomer = customers.find(
        (c) => String(c.customerId ?? c.id) === String(customerId)
      );
      const payload = {
        customerId,
        items,
      };
      const quoteResponse = await requestOrderQuote(payload);

      onQuoteReady({
        ...quoteResponse,
        customerId: payload.customerId,
        customerName:
          quoteResponse.customerName ||
          selectedCustomer?.fullName ||
          selectedCustomer?.name ||
          `Customer #${payload.customerId}`,
        items: payload.items,
      });
    } catch (err) {
      setError(err.message || "Unable to calculate quote.");
    } finally {
      setSubmittingQuote(false);
    }
  }

  return (
    <section className="panel">
      <div className="panel-top">
        <div>
          <h2>Create Order</h2>
          <p className="subtitle">
            Choose a customer, add products, then request a quote.
          </p>
        </div>
      </div>

      {error && (
        <div className="error-banner" role="alert" style={{ marginBottom: "1rem" }}>
          {error}
        </div>
      )}

      <form onSubmit={handleRequestQuote} className="order-builder-form">
        <div className="form-group">
          <label htmlFor="order-customer">Customer *</label>
          <select
            id="order-customer"
            className="select-input"
            value={customerId}
            disabled={loadingCustomers || submittingQuote}
            onChange={(e) => {
              setCustomerId(e.target.value);
              setError("");
            }}
          >
            {loadingCustomers ? (
              <option value="">Loading customers...</option>
            ) : customers.length === 0 ? (
              <option value="">No customers available</option>
            ) : (
              customers.map((c) => {
                const cid = c.customerId ?? c.id;
                return (
                  <option key={cid} value={cid}>
                    {c.fullName ?? c.name}
                  </option>
                );
              })
            )}
          </select>
        </div>

        <div className="line-items-header">
          <span className="section-label">Products</span>
        </div>

        {orderableProducts.length === 0 ? (
          <p className="muted">No active products with available stock.</p>
        ) : (
          <div className="line-items-box">
            <div className="line-grid-head">
              <span>Product</span>
              <span>Unit price</span>
              <span>Qty</span>
              <span>Line total</span>
              <span />
            </div>

            {rows.map((row) => {
              const product = getProductById(row.productId);
              const unitPrice = Number(product?.unitPrice ?? product?.price ?? 0);
              const stockQty = Number(product?.stockQuantity ?? product?.stock ?? 0);
              const qtyNum = Number(row.quantity);
              const lineTotal =
                Number.isFinite(qtyNum) && qtyNum > 0 ? unitPrice * qtyNum : 0;

              return (
                <div key={row.rowId} className="line-grid-row">
                  <div className="line-product-cell">
                    <select
                      aria-label="Product"
                      className="select-input"
                      value={row.productId}
                      disabled={submittingQuote}
                      onChange={(e) =>
                        handleRowChange(row.rowId, "productId", e.target.value)
                      }
                    >
                      {orderableProducts.map((p) => {
                        const pid = p.productId ?? p.id;
                        const pStock = Number(p.stockQuantity ?? p.stock ?? 0);
                        return (
                          <option key={pid} value={pid}>
                            {p.name} ({p.sku})
                          </option>
                        );
                      })}
                    </select>
                    <small className="stock-hint">{stockQty} in stock</small>
                  </div>

                  <div className="mono-cell">R{unitPrice.toFixed(2)}</div>

                  <div>
                    <input
                      aria-label="Quantity"
                      type="number"
                      min="1"
                      max={Math.max(1, stockQty)}
                      step="1"
                      className="qty-input"
                      value={row.quantity}
                      disabled={submittingQuote}
                      onChange={(e) =>
                        handleRowChange(row.rowId, "quantity", e.target.value)
                      }
                    />
                  </div>

                  <div className="line-total-cell">R{lineTotal.toFixed(2)}</div>

                  <div>
                    <button
                      type="button"
                      className="btn-icon-remove"
                      aria-label="Remove product row"
                      disabled={submittingQuote || rows.length === 1}
                      onClick={() => handleRemoveRow(row.rowId)}
                    >
                      ×
                    </button>
                  </div>
                </div>
              );
            })}

            <div className="add-row-bar">
              <button
                type="button"
                className="btn-secondary"
                disabled={submittingQuote}
                onClick={handleAddRow}
              >
                + Add product
              </button>
            </div>
          </div>
        )}

        <p className="footnote" style={{ marginTop: 0 }}>
          Quantity must be 1 or more and cannot exceed stock. Only active products can be added.[cite: 1, 2]
        </p>

        <div className="form-actions-right">
          {onCancel && (
            <button
              type="button"
              className="btn-secondary"
              disabled={submittingQuote}
              onClick={onCancel}
            >
              Cancel
            </button>
          )}
          <button
            type="submit"
            className="btn-primary"
            disabled={submittingQuote || orderableProducts.length === 0}
          >
            {submittingQuote ? "Calculating Quote..." : "Request Quote"}
          </button>
        </div>
      </form>
    </section>
  );
}