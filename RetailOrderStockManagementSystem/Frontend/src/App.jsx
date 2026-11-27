import { useCallback, useEffect, useMemo, useState } from 'react';
import {
  createBrowserRouter,
  Link,
  Navigate,
  NavLink,
  Route,
  Routes,
  useLocation,
  useNavigate,
} from 'react-router';
import Customers from './pages/Customers.jsx';
import Products from './pages/Products.jsx';
import ProductManagement from './pages/ProductManagement.jsx';
import CreateOrder from './pages/CreateOrder.jsx';
import QuoteSummary from './pages/QuoteSummary.jsx';
import Orders from './pages/Orders.jsx';
import OrderDetails from './pages/OrderDetails.jsx';
import './App.css';
import Login from './pages/Login.jsx';
import Reports from './pages/Reports.jsx';
import { AuthProvider, useAuth } from './auth/authContext.jsx';

function AppWorkspace({ selectedProduct, setSelectedProduct }) {
  const { token } = useAuth();
  const location = useLocation();
  const navigate = useNavigate();

  const [products, setProducts] = useState([]);
  const [loadingProducts, setLoadingProducts] = useState(true);
  const [productsError, setProductsError] = useState('');

  const [customers, setCustomers] = useState([]);
  const [quoteDraft, setQuoteDraft] = useState(null);
  const [submittingOrder, setSubmittingOrder] = useState(false);

  const fetchProducts = useCallback(async () => {
    setLoadingProducts(true);
    setProductsError('');
    try {
      const headers = {};
      if (token) {
        headers.Authorization = `Bearer ${token}`;
      }
      const response = await fetch('/api/products', { headers });
      if (!response.ok) {
        const problem = await response.json().catch(() => null);
        setProductsError(problem?.message || 'Unable to load products.');
        return;
      }
      const data = await response.json();
      setProducts(Array.isArray(data) ? data : data?.items ?? []);
    } catch (err) {
      console.error(err);
      setProductsError('Unable to load products. Please try again.');
    } finally {
      setLoadingProducts(false);
    }
  }, [token]);

  useEffect(() => {
    let ignore = false;

    async function loadInitialData() {
      const headers = {};
      if (token) {
        headers.Authorization = `Bearer ${token}`;
      }

      try {
        const [productsRes, customersRes] = await Promise.all([
          fetch('/api/products', { headers }),
          fetch('/api/customers', { headers }),
        ]);

        if (ignore) return;

        if (!productsRes.ok) {
          const problem = await productsRes.json().catch(() => null);
          setProductsError(problem?.message || 'Unable to load products.');
        } else {
          const productData = await productsRes.json();
          if (!ignore) {
            setProducts(
              Array.isArray(productData) ? productData : productData?.items ?? []
            );
            setProductsError('');
          }
        }

        if (customersRes.ok) {
          const customerData = await customersRes.json();
          if (!ignore) {
            const list = Array.isArray(customerData)
              ? customerData
              : customerData?.items ?? [];
            setCustomers(list.filter((c) => c.isActive !== false));
          }
        }
      } catch (err) {
        if (ignore) return;
        console.error(err);
        setProductsError('Unable to load products. Please try again.');
      } finally {
        if (!ignore) {
          setLoadingProducts(false);
        }
      }
    }

    loadInitialData();

    return () => {
      ignore = true;
    };
  }, [token]);

  const categories = useMemo(() => {
    const unique = new Set(
      products
        .map((p) => String(p?.category ?? '').trim())
        .filter((c) => c && c !== 'ALL')
    );
    return Array.from(unique).sort((a, b) => a.localeCompare(b));
  }, [products]);

  async function handleGetQuote(orderRequest) {
    const headers = { 'Content-Type': 'application/json' };
    if (token) {
      headers.Authorization = `Bearer ${token}`;
    }

    const response = await fetch('/api/orders/quote', {
      method: 'POST',
      headers,
      body: JSON.stringify(orderRequest),
    });

    if (!response.ok) {
      const problem = await response.json().catch(() => null);
      throw new Error(problem?.message || 'Unable to calculate order quote.');
    }

    const quoteData = await response.json();
    const backendQuoteItems =
      quoteData.items ?? quoteData.Items ?? quoteData.lines ?? [];

    setQuoteDraft({
      ...quoteData,
      customerId: quoteData.customerId ?? orderRequest.customerId,
      customerName: quoteData.customerName ?? orderRequest.customerName,
      items: backendQuoteItems,
      requestItems: orderRequest.items,
    });
    navigate('/quote-summary');
  }

  async function handleSubmitOrder() {
    if (!quoteDraft) return;

    setSubmittingOrder(true);
    try {
      const headers = { 'Content-Type': 'application/json' };
      if (token) {
        headers.Authorization = `Bearer ${token}`;
      }

      const response = await fetch('/api/orders', {
        method: 'POST',
        headers,
        body: JSON.stringify({
          customerId: quoteDraft.customerId,
          items: quoteDraft.requestItems ?? quoteDraft.items,
        }),
      });

      if (!response.ok) {
        const problem = await response.json().catch(() => null);
        throw new Error(problem?.message || 'Unable to submit the order.');
      }

      setQuoteDraft(null);
      await fetchProducts();
      navigate('/orders');
    } finally {
      setSubmittingOrder(false);
    }
  }

  const activeProduct = location.state?.product ?? selectedProduct;
  const editorKey = activeProduct?.productId ?? activeProduct?.id ?? 'new';

  return (
    <div className="app-content">
      <Routes>
        <Route path="/orders/:orderId" element={<OrderDetails />} />
        <Route path="/orders" element={<Orders />} />
        <Route path="/" element={<Navigate to="/customers" replace />} />
        <Route path="/customers" element={<Customers />} />
        <Route
          path="/products"
          element={
            <Products
              products={products}
              loading={loadingProducts}
              error={productsError}
              onRefresh={fetchProducts}
              onEditProduct={(product) => setSelectedProduct(product)}
            />
          }
        />
        <Route
          path="/product-management"
          element={
            <ProductManagement
              key={editorKey}
              products={products}
              categories={categories}
              selectedProduct={activeProduct}
              onSelectProduct={(product) => setSelectedProduct(product)}
              loading={loadingProducts}
              onRefresh={fetchProducts}
              onSaved={() => {
                setSelectedProduct(null);
                fetchProducts();
                navigate('/product-management', { replace: true, state: null });
              }}
              onCancel={() => {
                setSelectedProduct(null);
                navigate('/products', { state: null });
              }}
            />
          }
        />
        <Route
          path="/create-order"
          element={
            <CreateOrder
              customers={customers}
              products={products}
              onGetQuote={handleGetQuote}
              onCancel={() => {
                setQuoteDraft(null);
                navigate('/orders');
              }}
            />
          }
        />
        <Route
          path="/quote-summary"
          element={
            <QuoteSummary
              quote={quoteDraft}
              products={products}
              onSubmitOrder={handleSubmitOrder}
              onBack={() => navigate('/create-order')}
              submitting={submittingOrder}
            />
          }
        />
        <Route path="/login" element={<Login />} />
        <Route path="/reports" element={<Reports />} />
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
  );
}

export default function App() {
  const [selectedProduct, setSelectedProduct] = useState(null);

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

          <NavLink to="/products" className="sidebar-link">
            Products
          </NavLink>

          <NavLink to="/product-management" className="sidebar-link">
            Product Management
          </NavLink>

          <NavLink to="/create-order" className="sidebar-link">
            Create Order
          </NavLink>

          <NavLink to="/quote-summary" className="sidebar-link">
            Quote Summary
          </NavLink>

          <NavLink to="/orders" className="sidebar-link">
            Orders
          </NavLink>

          <NavLink to="/reports" className="sidebar-link">
            Reports
          </NavLink>
        </nav>
      </aside>

      <AuthProvider>
        <AppWorkspace
          selectedProduct={selectedProduct}
          setSelectedProduct={setSelectedProduct}
        />
      </AuthProvider>
    </div>
  );
}

/* eslint-disable react-refresh/only-export-components */
export const router = createBrowserRouter([
  {
    path: '/*',
    element: <App />,
  },
]);