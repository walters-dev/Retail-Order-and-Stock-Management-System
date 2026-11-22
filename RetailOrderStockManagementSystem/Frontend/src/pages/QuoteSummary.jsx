import { useState } from "react";
import { createOrder } from "../services/api";

function normalizeQuoteLines(quote) {
  const rawLines = quote?.lines ?? quote?.lineItems ?? quote?.items ?? [];
  return rawLines.map((line, index) => {
    const qty = Number(line.quantity ?? 0);
    const unitPrice = Number(line.unitPrice ?? line.price ?? 0);
    const lineTotal =
      line.lineTotal != null ? Number(line.lineTotal) : qty * unitPrice;

    return {
      key: line.productId ?? index,
      productName:
        line.productName ?? line.name ?? `Product #${line.productId ?? index + 1}`,
      quantity: qty,
      unitPrice,
      lineTotal,
    };
  });
}

export default function QuoteSummary({
  quote = null,
  onBack,
  onOrderSubmitted,
}) {
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState("");

  if (!quote) {
    return (
      <section className="panel">
        <h2>Quote Summary</h2>
        <p className="subtitle">
          Review before submitting. Nothing is saved yet.[cite: 1, 2]
        </p>
        <p className="muted" style={{ marginTop: "1rem" }}>
          No active quote to review. Start from <strong>Create Order</strong> and click{" "}
          <strong>Request Quote</strong>.[cite: 1, 2]
        </p>
        {onBack && (
          <div className="form-actions-right">
            <button type="button" className="btn-primary" onClick={onBack}>
              Go to Create Order
            </button>
          </div>
        )}
      </section>
    );
  }

  const lines = normalizeQuoteLines(quote);
  const subtotal = Number(
    quote.subtotal ?? lines.reduce((sum, l) => sum + l.lineTotal, 0)
  );
  const discountPercent = Number(
    quote.discountPercent ?? (subtotal >= 2000 ? 10 : 0)
  );
  const discountAmount = Number(
    quote.discountAmount ?? subtotal * (discountPercent / 100)
  );
  const finalTotal = Math.max(
    0,
    Number(quote.finalTotal ?? subtotal - discountAmount)
  );
  const hasDiscount = discountAmount > 0 || discountPercent > 0;

  async function handleSubmitOrder() {
    setError("");
    setSubmitting(true);

    try {
      const payload = {
        customerId: quote.customerId,
        items: (quote.items || []).map((item) => ({
          productId: Number(item.productId),
          quantity: Number(item.quantity),
        })),
      };

      const createdOrder = await createOrder(payload);
      if (onOrderSubmitted) {
        await onOrderSubmitted(createdOrder);
      }
    } catch (err) {
      setError(err.message || "Failed to submit order.");
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <section className="panel">
      <div className="panel-top">
        <div>
          <h2>Quote Summary</h2>
          <p className="subtitle">
            Review before submitting. Nothing is saved yet.[cite: 1, 2]
          </p>
        </div>
      </div>

      <p className="quote-customer">
        Customer: <strong>{quote.customerName}</strong>
      </p>

      {error && (
        <div className="error-banner" role="alert" style={{ marginBottom: "1rem" }}>
          {error}
        </div>
      )}

      <div className="table-card">
        <table>
          <thead>
            <tr>
              <th>Product</th>
              <th>Qty</th>
              <th>Unit price</th>
              <th>Line total</th>
            </tr>
          </thead>
          <tbody>
            {lines.map((line) => (
              <tr key={line.key}>
                <td>{line.productName}</td>
                <td>{line.quantity}</td>
                <td>R{line.unitPrice.toFixed(2)}</td>
                <td>R{line.lineTotal.toFixed(2)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      <div className="quote-breakdown">
        <div className="discount-callout">
          {hasDiscount ? (
            <>
              <strong>{discountPercent}% discount applied</strong>
              <span>Subtotal is R2,000 or more.[cite: 1, 2]</span>
            </>
          ) : (
            <>
              <strong>Standard pricing</strong>
              <span>10% discount applies when subtotal is R2,000 or more.[cite: 2]</span>
            </>
          )}
        </div>

        <dl className="totals-summary">
          <div className="totals-row">
            <dt>Subtotal</dt>
            <dd>R{subtotal.toFixed(2)}</dd>
          </div>
          <div className="totals-row">
            <dt>Discount ({discountPercent}%)</dt>
            <dd className={hasDiscount ? "discount-text" : ""}>
              {hasDiscount ? `−R${discountAmount.toFixed(2)}` : "R0.00"}
            </dd>
          </div>
          <div className="totals-row totals-final">
            <dt>Final Total</dt>
            <dd>R{finalTotal.toFixed(2)}</dd>
          </div>
        </dl>
      </div>

      <p className="footnote">
        Submitting saves the order as <strong>Pending</strong>. Stock is not reduced until it is
        confirmed.[cite: 1, 2]
      </p>

      <div className="form-actions-right">
        {onBack && (
          <button
            type="button"
            className="btn-secondary"
            disabled={submitting}
            onClick={onBack}
          >
            Back
          </button>
        )}
        <button
          type="button"
          className="btn-primary"
          disabled={submitting}
          onClick={handleSubmitOrder}
        >
          {submitting ? "Submitting..." : "Submit Order"}
        </button>
      </div>
    </section>
  );
}