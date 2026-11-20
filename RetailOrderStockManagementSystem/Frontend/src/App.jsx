import { Link, Navigate, NavLink, Route, Routes } from 'react-router'
import Customers from './pages/Customers.jsx'
import Orders from './pages/Orders.jsx'
import OrderDetails from './pages/OrderDetails.jsx'
import './App.css'

export default function App() {
  return (
    <div className="app-layout">
      <aside className="app-sidebar">
        <p className="sidebar-brand">
          Retail Order
          <br />
          &amp; Stock
        </p>

        <nav aria-label="Main navigation">
          <NavLink to="/customers" className="sidebar-link">
            Customers
          </NavLink>

          <NavLink to="/orders" className="sidebar-link">
            Orders
          </NavLink>
        </nav>
      </aside>

      <div className="app-content">
        <Routes>
          <Route
            path="/orders/:orderId"
            element={<OrderDetails />}/>
          <Route path="/orders" element={<Orders />} />
          <Route
            path="/"
            element={<Navigate to="/customers" replace />}
          />

          <Route
            path="/customers"
            element={<Customers />}
          />

          <Route
            path="*"
            element={
              <main>
                <h1>Page not found</h1>
                <Link to="/customers">Go to Customers</Link>
              </main>
            }
          />
        </Routes>
      </div>
    </div>
  )
}