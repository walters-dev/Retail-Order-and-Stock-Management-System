import { useState } from 'react';
import './CreateOrder.css';

export default function QuoteSummary({
  quote, // The validated payload passed from parent CreateOrder handler
  onSubmitOrder,
  onBack,
  submitting = false,
}) {
  const [error, setError] = useState('');

  if (!quote) {
    return (
      <section className="order-builder">
        <div className="order-card">
          <p className="order-footnote">No quote generated yet. Please create an order first.</p>
        </div>
      </section>
    );
  }

  async function handleConfirmSubmit() {
    setError('');
    try {
      await onSubmitOrder();
    } catch (err) {
      setError(err?.message || 'Failed to submit the order. Please try again.');
    }
  }

  const discountPercent = quote.discountPercent || 0;
  const isDiscounted = discountPercent > 0;

  return (
    <section className="order-builder">
      <div className="order-card">
        <div className="order-header">
          <div>
            <h1 className="order-title">Quote Summary</h1>
            <p className="order-subtitle">Review before submitting. Nothing is saved yet.</p>
          </div>
        </div>

        {error && (
          <div className="order-error-banner" role="alert">
            {error}
          </div>
        )}

        <div className="quote-summary-meta">
          Customer: <strong>{quote.customerName}</strong>
        </div>

        <div className="order-table-wrapper">
          <table className="order-table">
            <thead>
              <tr>
                <th>Product</th>
                <th>Qty</th>
                <th>Unit price</th>
                <th>Line total</th>
              </tr>
            </thead>
            <tbody>
              {quote.lines?.map((line, idx) => (
                <tr key={idx}>
                  <td>{line.productName || 'Unknown Product'}</td>
                  <td>{line.quantity}</td>
                  <td>R{Number(line.unitPrice || 0).toFixed(2)}</td>
                  <td style={{ fontWeight: 600 }}>R{Number(line.lineTotal || 0).toFixed(2)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {isDiscounted && (
          <div className="quote-highlight-box">
            <h3 className="quote-highlight-title">{discountPercent}% discount applied</h3>
            <p className="quote-highlight-desc">Subtotal is R2,000 or more.</p>
          </div>
        )}

        <div className="quote-total-grid">
          <div className="quote-total-label">Subtotal</div>
          <div className="quote-total-value">R{Number(quote.subtotal || 0).toFixed(2)}</div>

          {isDiscounted && (
            <>
              <div className="quote-total-label">Discount ({discountPercent}%)</div>
              <div className="quote-total-value quote-total-discount">-R{Number(quote.discountAmount || 0).toFixed(2)}</div>
            </>
          )}

          <div className="quote-total-final-label">Final Total</div>
          <div className="quote-total-final-value">R{Number(quote.finalTotal || 0).toFixed(2)}</div>
        </div>

        <div className="order-footer">
          <p className="order-footnote">
            Submitting saves the order as Pending. Stock is not reduced until it is confirmed.
          </p>
          <div className="order-actions">
            <button
              type="button"
              className="order-btn-cancel"
              onClick={onBack}
              disabled={submitting}
            >
              Back
            </button>
            <button
              type="button"
              className="order-btn-submit"
              onClick={handleConfirmSubmit}
              disabled={submitting}
            >
              {submitting ? 'Submitting...' : 'Submit Order'}
            </button>
          </div>
        </div>
      </div>
    </section>
  );
}