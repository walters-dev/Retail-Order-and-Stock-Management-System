import { useEffect, useState } from 'react';
import './CreateOrder.css';

export default function CreateOrder({
  customers: initialCustomers,
  products = [],
  onGetQuote,
  onCancel,
}) {
  const [customers, setCustomers] = useState(
    Array.isArray(initialCustomers) ? initialCustomers : []
  );
  const [loadingCustomers, setLoadingCustomers] = useState(
    !Array.isArray(initialCustomers) || initialCustomers.length === 0
  );
  const [customerLoadError, setCustomerLoadError] = useState('');
  const [selectedCustomerId, setSelectedCustomerId] = useState('');
  const [items, setItems] = useState([{ productId: '', quantity: 1 }]);
  const [error, setError] = useState('');
  const [isRequestingQuote, setIsRequestingQuote] = useState(false);

  useEffect(() => {
    const controller = new AbortController();

    async function loadCustomers() {
      setLoadingCustomers(true);
      setCustomerLoadError('');
      try {
        const response = await fetch('/api/customers', {
          signal: controller.signal,
        });

        if (!response.ok) {
          throw new Error(`Customer request failed (${response.status})`);
        }

        const data = await response.json();

        if (controller.signal.aborted) return;

        const list = Array.isArray(data) ? data : data?.items ?? [];
        setCustomers(
          list.map((customer) => ({
            ...customer,
            id: customer.customerId ?? customer.id,
          }))
        );
      } catch (err) {
        if (controller.signal.aborted) return;

        console.error(err);
        setCustomerLoadError(
          'Customers could not be loaded. Refresh to try again.'
        );
      } finally {
        if (!controller.signal.aborted) {
          setLoadingCustomers(false);
        }
      }
    }

    loadCustomers();

    return () => controller.abort();
  }, []);

  const activeCustomers = customers.filter((c) => c.isActive !== false);
  const activeProducts = products.filter((p) => p.isActive !== false);

  function handleAddProductRow() {
    setItems((prev) => [...prev, { productId: '', quantity: 1 }]);
    setError('');
  }

  function handleRemoveProductRow(index) {
    if (items.length === 1) {
      setError('An order must contain at least one product row.');
      return;
    }
    setItems((prev) => prev.filter((_, i) => i !== index));
    setError('');
  }

  function handleFieldChange(index, field, value) {
    setItems((prev) =>
      prev.map((item, idx) => {
        if (idx !== index) return item;
        if (field === 'productId') {
          return { ...item, productId: value };
        }
        if (field === 'quantity') {
          const parsed = Number(value);
          return {
            ...item,
            quantity: value === '' ? '' : Number.isNaN(parsed) ? '' : parsed,
          };
        }
        return item;
      })
    );
    setError('');
  }

  async function handleSubmitQuoteRequest(e) {
    e.preventDefault();
    setError('');

    const selectedId = String(selectedCustomerId).trim();
    if (!selectedId) {
      setError('Please select an active customer.');
      return;
    }

    const customer = activeCustomers.find(
      (c) => String(c.customerId ?? c.id ?? '').trim() === selectedId
    );
    if (!customer) {
      setError('Selected customer is invalid.');
      return;
    }

    const validItems = [];
    for (const item of items) {
      const itemProductId = String(item.productId ?? '').trim();
      if (!itemProductId) {
        setError('Please select a product for all added lines.');
        return;
      }

      const product = products.find(
        (p) => String(p.productId ?? p.id ?? '').trim() === itemProductId
      );
      if (!product) {
        setError('Selected product is invalid.');
        return;
      }

      const qty = Number(item.quantity);
      if (!Number.isInteger(qty) || qty < 1) {
        setError(
          `Quantity for "${product.name}" must be a positive whole number.`
        );
        return;
      }

      const stockQty = Number(product.stockQuantity ?? 0);
      if (qty > stockQty) {
        setError(
          `Requested quantity (${qty}) for "${product.name}" exceeds available stock (${stockQty}).`
        );
        return;
      }

      validItems.push({
        productId: product.productId ?? product.id,
        quantity: qty,
      });
    }

    const customerId = customer.customerId ?? customer.id;
    const customerName = customer.fullName ?? customer.name ?? '';

    setIsRequestingQuote(true);
    try {
      await onGetQuote({
        customerId,
        customerName,
        items: validItems,
      });
    } catch (err) {
      setError(err?.message || 'Unable to request quote. Please try again.');
    } finally {
      setIsRequestingQuote(false);
    }
  }

  const displayedError = error || customerLoadError;

  return (
    <section className="order-builder">
      <div className="order-card">
        <div className="order-header">
          <div>
            <h1 className="order-title">Create Order</h1>
            <p className="order-subtitle">
              Choose a customer, add products, then request a quote.
            </p>
          </div>
        </div>

        {displayedError && (
          <div className="order-error-banner" role="alert">
            {displayedError}
          </div>
        )}

        <form onSubmit={handleSubmitQuoteRequest}>
          <div className="order-form-group">
            <label htmlFor="order-cust">Customer *</label>
            <select
              id="order-cust"
              className="order-select"
              value={selectedCustomerId}
              onChange={(e) => {
                setSelectedCustomerId(e.target.value);
                setError('');
              }}
              disabled={isRequestingQuote || loadingCustomers}
            >
              <option value="">
                {loadingCustomers
                  ? 'Loading customers...'
                  : 'Select an active customer...'}
              </option>
              {activeCustomers.map((c, idx) => {
                const custId = c.customerId ?? c.id ?? idx;
                const custName = c.fullName ?? c.name ?? 'Customer';
                return (
                  <option key={custId} value={custId}>
                    {custName}
                    {c.mobileNumber ? ` (${c.mobileNumber})` : ''}
                  </option>
                );
              })}
            </select>
          </div>

          <div className="order-table-wrapper">
            <table className="order-table">
              <thead>
                <tr>
                  <th>Product</th>
                  <th>Unit price</th>
                  <th>Qty</th>
                  <th>Line total</th>
                  <th>Action</th>
                </tr>
              </thead>
              <tbody>
                {items.map((item, index) => {
                  const itemProdId = String(item.productId ?? '').trim();
                  const targetProd = activeProducts.find(
                    (p) => String(p.productId ?? p.id ?? '').trim() === itemProdId
                  );
                  const linePrice = targetProd
                    ? Number(targetProd.unitPrice ?? targetProd.price ?? 0)
                    : 0;
                  const qtyNum = Number(item.quantity);
                  const lineTotal =
                    targetProd && qtyNum > 0 ? linePrice * qtyNum : 0;
                  const stockQty = targetProd
                    ? Number(targetProd.stockQuantity ?? 0)
                    : 0;

                  return (
                    <tr key={index}>
                      <td style={{ width: '40%' }}>
                        <select
                          className="order-select"
                          value={item.productId}
                          onChange={(e) =>
                            handleFieldChange(index, 'productId', e.target.value)
                          }
                          disabled={isRequestingQuote}
                        >
                          <option value="">Select a product...</option>
                          {activeProducts.map((p, pIdx) => {
                            const prodId = p.productId ?? p.id ?? pIdx;
                            return (
                              <option key={prodId} value={prodId}>
                                {p.name}
                              </option>
                            );
                          })}
                        </select>
                        {targetProd && (
                          <span
                            className={`order-stock-label ${
                              stockQty === 0 ? 'order-stock-error' : ''
                            }`}
                          >
                            {stockQty} in stock
                          </span>
                        )}
                      </td>
                      <td>R{linePrice.toFixed(2)}</td>
                      <td>
                        <input
                          type="number"
                          className="order-qty-input"
                          min="1"
                          step="1"
                          value={item.quantity}
                          onChange={(e) =>
                            handleFieldChange(index, 'quantity', e.target.value)
                          }
                          disabled={isRequestingQuote}
                        />
                      </td>
                      <td style={{ fontWeight: 600 }}>
                        R{lineTotal.toFixed(2)}
                      </td>
                      <td>
                        <button
                          type="button"
                          className="order-remove-btn"
                          onClick={() => handleRemoveProductRow(index)}
                          title="Remove Line"
                          disabled={isRequestingQuote}
                        >
                          ×
                        </button>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>

          <button
            type="button"
            className="order-add-btn"
            onClick={handleAddProductRow}
            disabled={isRequestingQuote}
          >
            + Add product
          </button>

          <div className="order-footer">
            <p className="order-footnote">
              Quantity must be 1 or more and cannot exceed stock. Only active
              products can be added.
            </p>
            <div className="order-actions">
              <button
                type="button"
                className="order-btn-cancel"
                onClick={onCancel}
                disabled={isRequestingQuote}
              >
                Cancel
              </button>
              <button
                type="submit"
                className="order-btn-submit"
                disabled={isRequestingQuote || loadingCustomers}
              >
                {isRequestingQuote ? 'Requesting...' : 'Request Quote'}
              </button>
            </div>
          </div>
        </form>
      </div>
    </section>
  );
}