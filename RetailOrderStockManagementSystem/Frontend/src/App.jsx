import { createBrowserRouter, Link, Navigate, NavLink, Route, Routes } from 'react-router-dom';
import Customers from './pages/Customers.jsx';
import Orders from './pages/Orders.jsx';
import OrderDetails from './pages/OrderDetails.jsx';
import './App.css';
import Login from './pages/Login.jsx';
import Reports from './pages/Reports.jsx';
import Registry from './pages/Register.jsx';
import { AuthProvider } from './auth/authContext.jsx';
import {ProtectedRoute, ProtectedAdminStaffRoute, ProtectedManagerAdminRoute} from './auth/ProtectedRoute.jsx'

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

          <NavLink to="/reports" className="sidebar-link">
            Reports
          </NavLink>

          <NavLink to="/register" className="sidebar-link">
            Register
          </NavLink>
        </nav>
      </aside>

      <div className="app-content">
        
        <AuthProvider>
          
          <Routes>
            <Route
              path="/login"
              element={<Login />}
            />
            {/* <Route element={<ProtectedRoute/>}> */}
            <Route element={<ProtectedAdminStaffRoute/>}>
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
            
            </Route>
            <Route id="ManagerAdmin" element={<ProtectedManagerAdminRoute/>}>
            <Route
              path="/reports"
              element={<Reports />}
            />
            <Route
              path="/register"
              element={<Registry/>}/>
            
            </Route>
            
            {/* </Route> */}
           
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