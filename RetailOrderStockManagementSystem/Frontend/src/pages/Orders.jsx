import { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import { useAuth } from '../auth/authContext.jsx'
import './Orders.css'
// const TOKEN_KEY = "token"




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
  const { token, role } = useAuth()

  const [selectedStatus, setSelectedStatus] = useState('All')
  const [page, setPage] = useState(1)
  const [pageSize, setPageSize] = useState(10)
  const [orders, setOrders] = useState([])
  // const [token, setToken] = useState(localStorage.getItem(TOKEN_KEY) || "");
  const [isLoading, setIsLoading] = useState(true)
  const [loadError, setLoadError] = useState('')

  useEffect(() => {
    const controller = new AbortController()

    async function loadOrders() {
      try {
        setIsLoading(true)
        setLoadError('')

        let url

        if (role === 'Admin') {
          url = '/api/orders'
        } else if (role === 'Staff') {
          url = '/api/orders/staff'
        } else {
          throw new Error('Your account cannot view orders.')
        }

        const response = await fetch(
          `${url}`, {
          signal: controller.signal,
          headers: {
            "Content-Type": "application/json",
            'Authorization': (`Bearer ${token}`)
          }
        }

        )

        if (!response.ok) {
          throw new Error(`Order request failed (${response.status})`)
        }
        const data = await response.json()
        if (controller.signal.aborted) return

        setOrders(data)
      }
      catch (error) {
        if (controller.signal.aborted) return
        console.log(error)
        setLoadError('Orders Could Not Be Loaded. Refresh to try Again')
      } finally {
        if (!controller.signal.aborted) setIsLoading(false)
      }
    }
    loadOrders()
    return () => controller.abort()
  }, [token, role])


  const matchingOrders = orders.filter(
    (order) =>
      selectedStatus === 'All' ||
      order.status === selectedStatus
  )

  const totalPages = Math.max(
    1,
    Math.ceil(matchingOrders.length / pageSize)
  )

  const currentPage = Math.min(page, totalPages)
  const startIndex = (currentPage - 1) * pageSize

  const paginatedOrders = matchingOrders.slice(
    startIndex,
    startIndex + pageSize
  )

  if (isLoading) {
    return (
      <main>
        <h1>Orders</h1>
        <p role="status">Loading orders...</p>
      </main>
    )
  }

  if (loadError) {
    return (
      <main>
        <h1>Orders</h1>
        <p role="alert">{loadError}</p>
      </main>
    )
  }

  return (
    <main>
      <h1>Orders</h1>

      <section className="orders-panel" aria-label="Order list">


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
              onClick={() => {
                setSelectedStatus(status)
                setPage(1)
              }}
            >
              {status}
            </button>
          ))}
        </div>

        <div className="orders-table-wrapper">
          <table className="orders-table">
            <thead>
              <tr>
                <th scope="col">Receipt ID</th>
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

              {paginatedOrders.map((order) => (
                <tr key={order.orderId}>
                  <td>
                    <Link
                      to={`/orders/${order.orderId}`}
                      title={`View receipt #${order.receiptId}`}
                      className="order-view-link"
                    >
                      #{order.receiptId}
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
        <div className="pagination-controls">
          <label htmlFor="orders-page-size">Rows per page:</label>

          <select
            id="orders-page-size"
            value={pageSize}
            onChange={(event) => {
              setPageSize(Number(event.target.value))
              setPage(1)
            }}
          >
            <option value={10}>10</option>
            <option value={20}>20</option>
            <option value={50}>50</option>
          </select>

          <button
            type="button"
            onClick={() => setPage(currentPage - 1)}
            disabled={currentPage === 1}
          >
            Previous
          </button>

          <span aria-live="polite">
            Page {currentPage} of {totalPages}
          </span>

          <button
            type="button"
            onClick={() => setPage(currentPage + 1)}
            disabled={currentPage >= totalPages}
          >
            Next
          </button>
        </div>
      </section>
    </main>
  )
}
