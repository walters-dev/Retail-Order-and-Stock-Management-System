import { useState } from 'react'
import { Link } from 'react-router'
import { sampleOrders } from './sampleOrders.js'
import './Orders.css'

const statuses = [
  'All',
  'Pending',
  'Confirmed',
  'Completed',
  'Cancelled',
]

// Display amounts in South African rand.
const currencyFormatter = new Intl.NumberFormat('en-ZA', {
  style: 'currency',
  currency: 'ZAR',
})

export default function Orders() {
  const [selectedStatus, setSelectedStatus] = useState('All')

  const matchingOrders = sampleOrders.filter(
    (order) =>
      selectedStatus === 'All' ||
      order.status === selectedStatus
  )

  return (
    <main>
      <h1>Orders</h1>

      <section className="orders-panel" aria-label="Order list">
        <p className="orders-note">Sample data</p>

        <div
          className="order-filters"
          role="group"
          aria-label="Filter orders by status"
        >
          {statuses.map((status) => (
            <button
              key={status}
              type="button"
              className="order-filter"
              aria-pressed={selectedStatus === status}
              onClick={() => setSelectedStatus(status)}
            >
              {status}
            </button>
          ))}
        </div>

        <div className="orders-table-wrapper">
          <table className="orders-table">
            <thead>
              <tr>
                <th scope="col">Order ID</th>
                <th scope="col">Date</th>
                <th scope="col" className="order-total">
                  Total
                </th>
                <th scope="col">Status</th>
              </tr>
            </thead>

            <tbody>
              {matchingOrders.length === 0 && (
                <tr>
                  <td colSpan={4}>
                    No orders match this status.
                  </td>
                </tr>
              )}

              {matchingOrders.map((order) => (
                <tr key={order.orderId}>
                 <td>
                  <Link
                    to={`/orders/${order.orderId}`}
                    title={order.orderId}
                    className="order-view-link"
                >
                    {order.orderId.slice(0, 8)}
                  </Link>
                </td>

                  <td>{order.orderDate}</td>

                  <td className="order-total">
                    {currencyFormatter.format(order.finalTotal)}
                  </td>

                  <td>
                    <span
                      className={`order-status order-status-${order.status.toLowerCase()}`}
                    >
                      {order.status}
                    </span>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </section>
    </main>
  )
}