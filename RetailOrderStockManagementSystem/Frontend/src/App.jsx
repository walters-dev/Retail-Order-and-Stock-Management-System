import { createBrowserRouter, Link, Navigate, NavLink, Route, Routes } from 'react-router-dom';
import Customers from './pages/Customers.jsx';
import Orders from './pages/Orders.jsx';
import OrderDetails from './pages/OrderDetails.jsx';
import './App.css';
import Login from './pages/Login.jsx';
import Reports from './pages/Reports.jsx';
import { AuthProvider, useAuth } from './auth/authContext.jsx';
import logo from './assets/logo.png';
import LogoutButton from './components/LogoutButton.jsx';
import UserRoleTag from './components/UserRoleTag.jsx'
const role = localStorage.getItem('role');
// A small button that logs the user out.
// It is its own component because only components INSIDE <AuthProvider>
// can call useAuth().


export default function App() {
  return (

    <div className="app-layout">
      <aside className="app-sidebar">
        <img src={logo} alt="redAcademy" className="sidebar-logo" />




        <p className="sidebar-brand">
          Retail Order
          <br />
          &amp; Stock
        </p>

        <UserRoleTag />
        
        <nav aria-label="Main navigation">
          <NavLink to="/customers" className="sidebar-link">
            Customers
          </NavLink>

          <NavLink to="/orders" className="sidebar-link">
            Orders
          </NavLink>
          <NavLink to="/reports" className="sidebar-link">
            Reports
          </NavLink>

        </nav>
        <div className='logout-button'>
          <LogoutButton />
        </div>

      </aside>

      <div className="app-content">
        <AuthProvider>
          <Routes>
            <Route
              path="/orders/:orderId"
              element={<OrderDetails />} />
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
              path="/login"
              element={<Login />}
            />

            <Route
              path="/reports"
              element={<Reports />}
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
        </AuthProvider>
      </div>

    </div>



  )
}
// export const router = createBrowserRouter([
//   {
//     path : "/",
//     element: <Navigate to="/customers" replace />
//   },
//   {
//     path: "/orders/:orderId",
//     element: <OrderDetails />
//   },
//   {
//     path: "/customers",
//     element: <Customers />
//   },
//   {
//     path: "/login",
//     element: <Login />
//   },
//   {
//     path: "/reports",
//     element: <Reports />
//   },
//   {
//     path: "*",
//     element:
//       <main>
//         <h1>Page not found</h1>
//         <Link to="/customers">Go to Customers</Link>
//       </main>
//   }
// ])