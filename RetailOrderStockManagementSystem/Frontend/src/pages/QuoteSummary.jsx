import { useState } from 'react';
import './CreateOrder.css';

export default function QuoteSummary({
  quote,
  products = [],
  onSubmitOrder,
  onBack,
  submitting = false,
}) {
  const [error, setError] = useState('');

  if (!quote) {
    return (
      <section className="order-builder">
        <div className="order-card">
          <p className="order-footnote">
            No quote generated yet. Please create an order first.
          </p>
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

  const rawLines =
    quote.lines ??
    quote.lineItems ??
    quote.items ??
    quote.LineItems ??
    quote.Items ??
    [];

  const normalizedLines = Array.isArray(rawLines)
    ? rawLines.map((line) => {
        const lineProdId = String(
          line.productId ?? line.ProductId ?? line.id ?? ''
        ).trim();
        const matchedProduct = products.find(
          (p) => String(p.productId ?? p.id ?? '').trim() === lineProdId
        );

        const quantity = Number(line.quantity ?? line.Quantity ?? line.qty ?? 0);
        const unitPrice = Number(
          line.unitPrice ??
            line.UnitPrice ??
            line.price ??
            matchedProduct?.unitPrice ??
            matchedProduct?.price ??
            0
        );
        const rawLineTotal = line.lineTotal ?? line.LineTotal ?? line.total;
        const lineTotal =
          rawLineTotal !== undefined && rawLineTotal !== null
            ? Number(rawLineTotal)
            : quantity * unitPrice;

        const productName =
          line.productName ??
          line.ProductName ??
          line.name ??
          matchedProduct?.name ??
          'Unknown Product';

        return {
          productId: lineProdId,
          productName,
          quantity,
          unitPrice,
          lineTotal,
        };
      })
    : [];

  const customerName =
    quote.customerName ?? quote.CustomerName ?? 'Selected Customer';

  const subtotal = Number(
    quote.subtotal ??
      quote.Subtotal ??
      normalizedLines.reduce((sum, l) => sum + l.lineTotal, 0)
  );

  const rawDiscountPercent =
    quote.discountPercent ?? quote.DiscountPercent ?? 0;
  const discountPercent =
    rawDiscountPercent > 0 && rawDiscountPercent < 1
      ? Math.round(rawDiscountPercent * 100)
      : Number(rawDiscountPercent) || 0;

  const discountAmount = Number(
    quote.discountAmount ??
      quote.DiscountAmount ??
      (discountPercent > 0 ? subtotal * (discountPercent / 100) : 0)
  );

  const finalTotal = Number(
    quote.finalTotal ??
      quote.FinalTotal ??
      Math.max(0, subtotal - discountAmount)
  );

  const isDiscounted = discountPercent > 0 || discountAmount > 0;
  const discountLabel =
    quote.discountName ??
    quote.DiscountName ??
    (discountPercent === 15
      ? 'First Time Customer Discount'
      : discountPercent === 10
        ? 'Large Order Discount'
        : discountPercent === 5
          ? 'Loyalty Discount'
          : 'Discount');

  return (
    <section className="order-builder">
      <div className="order-card">
        <div className="order-header">
          <div>
            <h1 className="order-title">Quote Summary</h1>
            <p className="order-subtitle">
              Review before submitting. Nothing is saved yet.
            </p>
          </div>
        </div>

        {error && (
          <div className="order-error-banner" role="alert">
            {error}
          </div>
        )}

        <div className="quote-summary-meta">
          Customer: <strong>{customerName}</strong>
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
              {normalizedLines.length === 0 ? (
                <tr>
                  <td colSpan={4} className="order-footnote">
                    No line items returned in quote.
                  </td>
                </tr>
              ) : (
                normalizedLines.map((line, idx) => (
                  <tr key={line.productId || idx}>
                    <td>{line.productName}</td>
                    <td>{line.quantity}</td>
                    <td>R{Number(line.unitPrice || 0).toFixed(2)}</td>
                    <td style={{ fontWeight: 600 }}>
                      R{Number(line.lineTotal || 0).toFixed(2)}
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>

        {isDiscounted && (
          <div className="quote-highlight-box">
            <h3 className="quote-highlight-title">
              {discountPercent}% discount applied
            </h3>
            <p className="quote-highlight-desc">{discountLabel} (non-stacking).</p>
          </div>
        )}

        <div className="quote-total-grid">
          <div className="quote-total-label">Subtotal</div>
          <div className="quote-total-value">R{subtotal.toFixed(2)}</div>

          {isDiscounted && (
            <>
              <div className="quote-total-label">
                Discount ({discountPercent}%)
              </div>
              <div className="quote-total-value quote-total-discount">
                -R{discountAmount.toFixed(2)}
              </div>
            </>
          )}

          <div className="quote-total-final-label">Final Total</div>
          <div className="quote-total-final-value">
            R{finalTotal.toFixed(2)}
          </div>
        </div>

        <div className="order-footer">
          <p className="order-footnote">
            Submitting saves the order as Pending. Stock is not reduced until it
            is confirmed.
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