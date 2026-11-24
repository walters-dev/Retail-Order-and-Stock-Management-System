import { useState, useEffect } from 'react'
import { Link, useParams } from 'react-router'
import './Orders.css'
import OrderActions from './OrderActions.jsx' 

const currencyFormatter = new Intl.NumberFormat('en-ZA', {
  style: 'currency',
  currency: 'ZAR',
})

export default function OrderDetails() {
  const { orderId } = useParams()
  const [orders, setOrders] = useState(null)
  const [isLoading, setIsLoading] = useState(true)
  const [loadError, setLoadError] = useState('')

   // Load this order from the backend when the page opens or the ID changes.
   useEffect(() => {
    const controller = new AbortController()

    async function loadOrder() {
      try {
        const response = await fetch(`/api/orders/${orderId}`, {
          signal: controller.signal,
        })

        if (response.status === 404) throw new Error('Order not found.')
        if (!response.ok) {
          throw new Error(`Order request failed (${response.status})`)
        }

        const data = await response.json()
        if (controller.signal.aborted) return

        setOrder(data)
      } catch (error) {
        if (controller.signal.aborted) return

        console.error(error)
        setLoadError(error.message)
      } finally {
        if (!controller.signal.aborted) setIsLoading(false)
      }
    }

    loadOrder()
    return () => controller.abort()
  }, [orderId])

  if (isLoading) {
    return (
      <main>
        <h1>Order</h1>
        <p role="status">Loading order…</p>
      </main>
    )
  }

  if (loadError || !order) {
    return (
      <main>
        <h1>Order not found</h1>
        <p role="alert">{loadError || 'That order could not be loaded.'}</p>
        <Link to="/orders">Back to orders</Link>
      </main>
    )
  }


  // Use an empty array if items are missing.
  const items = order.items ?? []

  return (
    <main>
      <Link to="/orders" className="order-back-link">
        Back to orders
      </Link>

      <h1>Order {order.orderId.slice(0, 8)}</h1>

      <section className="orders-panel" aria-label="Order details">
        <p className="orders-note">Sample data</p>

        <div className="order-meta">
          <p>Order date: {order.orderDate}</p>

          <span
            className={`order-status order-status-${order.status.toLowerCase()}`}
          >
            {order.status}
          </span>
        </div>

        <OrderActions order={order} onStatusChanged={setOrder} />

        <section className="order-customer">
          <h2>Customer</h2>

          {order.customer ? (
            <>
              <p>
                <strong>{order.customer.fullName}</strong>
              </p>

              <p>{order.customer.mobileNumber}</p>

              {order.customer.email && (
                <p>{order.customer.email}</p>
              )}
            </>
          ) : (
            <p>Customer information is unavailable.</p>
          )}
        </section>

        <h2 className="order-section-title">Products</h2>

        <div className="orders-table-wrapper">
          <table className="orders-table">
            <thead>
              <tr>
                <th scope="col">Product</th>
                <th scope="col" className="order-number">
                  Quantity
                </th>
                <th scope="col" className="order-total">
                  Unit price
                </th>
                <th scope="col" className="order-total">
                  Line total
                </th>
              </tr>
            </thead>

            <tbody>
              {items.length === 0 && (
                <tr>
                  <td colSpan={4}>
                    No products in this order.
                  </td>
                </tr>
              )}

              {items.map((item) => (
                <tr key={item.orderLineItemId}>
                  <td>{item.productName}</td>

                  <td className="order-number">
                    {item.quantity}
                  </td>

                  <td className="order-total">
                    {currencyFormatter.format(item.unitPrice)}
                  </td>

                  <td className="order-total">
                    {currencyFormatter.format(item.lineTotal)}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        <dl className="order-summary order-details-totals">
          <dt>Subtotal</dt>
          <dd>
            {currencyFormatter.format(order.subtotal)}
          </dd>

          <dt>Discount ({order.discountPercent}%)</dt>
          <dd>
            {currencyFormatter.format(order.discountAmount)}
          </dd>

          <dt>Final total</dt>
          <dd>
            {currencyFormatter.format(order.finalTotal)}
          </dd>
        </dl>
      </section>
    </main>
  )
}