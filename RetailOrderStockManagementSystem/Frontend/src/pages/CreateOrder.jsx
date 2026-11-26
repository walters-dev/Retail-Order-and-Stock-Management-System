import { useState } from 'react';
import './CreateOrder.css';

export default function CreateOrder({
  customers = [],
  products = [],
  onGetQuote, // Handled by App.jsx to progress to Quote Summary
  onCancel,
}) {
  const [selectedCustomerId, setSelectedCustomerId] = useState('');
  const [items, setItems] = useState([
    { productId: '', quantity: 1 }
  ]);
  const [error, setError] = useState('');

  const activeProducts = products.filter(p => p.isActive);

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
          return { ...item, productId: Number(value) || '' };
        }
        if (field === 'quantity') {
          return { ...item, quantity: Number(value) || '' };
        }
        return item;
      })
    );
    setError('');
  }

  function handleSubmitQuoteRequest(e) {
    e.preventDefault();
    setError('');

    if (!selectedCustomerId) {
      setError('Please select an active customer.');
      return;
    }

    const validItems = [];
    for (const item of items) {
      if (!item.productId) {
        setError('Please select a product for all added lines.');
        return;
      }

      const product = products.find(p => p.productId === item.productId);
      if (!product) continue;

      if (!item.quantity || !Number.isInteger(item.quantity) || item.quantity < 1) {
        setError(`Quantity for "${product.name}" must be a positive whole number.`);
        return;
      }

      if (item.quantity > product.stockQuantity) {
        setError(
          `Requested quantity (${item.quantity}) for "${product.name}" exceeds available stock (${product.stockQuantity}).`
        );
        return;
      }

      validItems.push({
        productId: item.productId,
        quantity: item.quantity,
      });
    }

    const customer = customers.find(c => c.customerId === Number(selectedCustomerId));
    if (!customer) {
      setError('Selected customer is invalid.');
      return;
    }

    onGetQuote({
      customerId: customer.customerId,
      customerName: customer.fullName,
      items: validItems
    });
  }

  return (
    <section className="order-builder">
      <div className="order-card">
        <div className="order-header">
          <div>
            <h1 className="order-title">Create Order</h1>
            <p className="order-subtitle">Choose a customer, add products, then request a quote.</p>
          </div>
        </div>

        {error && (
          <div className="order-error-banner" role="alert">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmitQuoteRequest}>
          <div className="order-form-group">
            <label htmlFor="order-cust">Customer *</label>
            <select
              id="order-cust"
              className="order-select"
              value={selectedCustomerId}
              onChange={(e) => setSelectedCustomerId(e.target.value)}
            >
              <option value="">Select an active customer...</option>
              {customers.map((c) => (
                <option key={c.customerId} value={c.customerId}>
                  {c.fullName} ({c.mobileNumber})
                </option>
              ))}
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
                  const targetProd = activeProducts.find(p => p.productId === item.productId);
                  const linePrice = targetProd ? targetProd.unitPrice : 0;
                  const lineTotal = targetProd && item.quantity ? linePrice * item.quantity : 0;

                  return (
                    <tr key={index}>
                      <td style={{ width: '40%' }}>
                        <select
                          className="order-select"
                          value={item.productId}
                          onChange={(e) => handleFieldChange(index, 'productId', e.target.value)}
                        >
                          <option value="">Select a product...</option>
                          {activeProducts.map((p) => (
                            <option key={p.productId} value={p.productId}>
                              {p.name}
                            </option>
                          ))}
                        </select>
                        {targetProd && (
                          <span className={`order-stock-label ${targetProd.stockQuantity === 0 ? 'order-stock-error' : ''}`}>
                            {targetProd.stockQuantity} in stock
                          </span>
                        )}
                      </td>
                      <td>
                        R{linePrice.toFixed(2)}
                      </td>
                      <td>
                        <input
                          type="number"
                          className="order-qty-input"
                          min="1"
                          step="1"
                          value={item.quantity}
                          onChange={(e) => handleFieldChange(index, 'quantity', e.target.value)}
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
          >
            + Add product
          </button>

          <div className="order-footer">
            <p className="order-footnote">
              Quantity must be 1 or more and cannot exceed stock. Only active products can be added.
            </p>
            <div className="order-actions">
              <button
                type="button"
                className="order-btn-cancel"
                onClick={onCancel}
              >
                Cancel
              </button>
              <button
                type="submit"
                className="order-btn-submit"
              >
                Request Quote
              </button>
            </div>
          </div>
        </form>
      </div>
    </section>
  );
}