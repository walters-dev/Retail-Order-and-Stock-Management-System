import { useState, useEffect} from 'react'
import { Link } from 'react-router'
import './Orders.css'

// const response = await fetch(
//   'https://localhost:7041/api/customers', 
//   {signal: controller.signal})

  
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
  const[orders, setOrders] = useState([])
  const[isLoading, setIsLoading] = useState(true)
  const[loadError, setLoadError] = useState('')

useEffect(() =>{
  const controller = new AbortController()

  async function loadOrders(){
    try {
      const response = await fetch (
        '/api/customers',{signal:controller.signal})

      if (!response.ok) {
          throw new Error(`Order request failed (${response.status})`)
        }
      const data = await response.json()
      if (controller.signal.aborted) return

      setOrders(data)
    }
    catch (error) {
      if (controller.signal.aborted) return
      console.error(error)
      setLoadError('Orders Could Not Be Loaded. Refresh to try Again')
    } finally {
      if (!controller.signal.aborted) setIsLoading(false)
    }
  }
  loadOrders()
  return() => controller.abort()
}, [])


  const matchingOrders = orders.filter(
    (order) =>
      selectedStatus === 'All' ||
      order.status === selectedStatus
  )

  if (isLoading) {
    return(
      <main>
        <h1>Orders</h1>
        <p role="status">Loading orders...</p>
      </main>
    )
  }

  if(loadError){
    return(
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