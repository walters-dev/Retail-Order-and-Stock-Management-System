import { useCallback, useEffect, useState } from "react";
import { getProducts, login, USE_MOCK_API } from "./services/api";
import Products from "./pages/Products";
import ProductManagement from "./pages/ProductManagement";
import CreateOrder from "./pages/CreateOrder";
import QuoteSummary from "./pages/QuoteSummary";
import "./App.css";

export default function App() {
  const [accessToken, setAccessToken] = useState(
    () => USE_MOCK_API ? "mock" : sessionStorage.getItem("retailOrderAccessToken") || ""
  );
  const [role, setRole] = useState(
    () => USE_MOCK_API ? "Admin" : sessionStorage.getItem("retailOrderRole") || ""
  );
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [loginError, setLoginError] = useState("");
  const [loggingIn, setLoggingIn] = useState(false);
  const [activeScreen, setActiveScreen] = useState("products");
  const [products, setProducts] = useState([]);
  const [loadingProducts, setLoadingProducts] = useState(true);
  const [productsError, setProductsError] = useState("");
  const [editingProduct, setEditingProduct] = useState(null);
  const [quoteDraft, setQuoteDraft] = useState(null);
  const [submittedOrders, setSubmittedOrders] = useState([]);
  const [orderNotice, setOrderNotice] = useState("");

  const loadProducts = useCallback(async () => {
    if (!USE_MOCK_API && !sessionStorage.getItem("retailOrderAccessToken")) return;
    setLoadingProducts(true);
    setProductsError("");
    try {
      const data = await getProducts();
      setProducts(data);
    } catch (err) {
      setProductsError(err.message || "Failed to load products.");
    } finally {
      setLoadingProducts(false);
    }
  }, []);

  useEffect(() => {
    if (!accessToken) return undefined;

    let active = true;
    getProducts()
      .then((data) => {
        if (active) setProducts(data);
      })
      .catch((err) => {
        if (active) setProductsError(err.message || "Failed to load products.");
      })
      .finally(() => {
        if (active) setLoadingProducts(false);
      });

    return () => {
      active = false;
    };
  }, [accessToken]);

  async function handleLogin(event) {
    event.preventDefault();
    setLoginError("");
    setLoggingIn(true);

    try {
      const result = await login(username.trim(), password);
      sessionStorage.setItem("retailOrderAccessToken", result.token);
      sessionStorage.setItem("retailOrderRole", result.role);
      setAccessToken(result.token);
      setRole(result.role);
      setPassword("");
      setLoadingProducts(true);
      setProductsError("");
    } catch (err) {
      setLoginError(err.message || "Unable to sign in.");
    } finally {
      setLoggingIn(false);
    }
  }

  function handleLogout() {
    sessionStorage.removeItem("retailOrderAccessToken");
    sessionStorage.removeItem("retailOrderRole");
    setAccessToken("");
    setRole("");
    setProducts([]);
    setQuoteDraft(null);
    setSubmittedOrders([]);
    setOrderNotice("");
    setActiveScreen("products");
  }

  function handleSelectEditProduct(product) {
    setEditingProduct(product);
    setActiveScreen("product-mgmt");
  }

  function handleQuoteReady(quoteData) {
    setOrderNotice("");
    setQuoteDraft(quoteData);
    setActiveScreen("quote-summary");
  }

  async function handleOrderSubmitted(createdOrder) {
    setSubmittedOrders((prev) => [createdOrder, ...prev]);
    setQuoteDraft(null);
    setOrderNotice(
      `Order saved as ${createdOrder.status ?? "Pending"} (Total: R${Number(
        createdOrder.finalTotal ?? 0
      ).toFixed(2)}). Stock remains unchanged until confirmed.`
    );
    await loadProducts();
    setActiveScreen("create-order");
  }

  if (!USE_MOCK_API && !accessToken) {
    return (
      <main className="main-content">
        <section className="panel">
          <h2>Sign in</h2>
          <p className="subtitle">Sign in with a backend account to use the store workspace.</p>
          {loginError && (
            <div className="error-banner" role="alert" style={{ marginBottom: "1rem" }}>
              {loginError}
            </div>
          )}
          <form onSubmit={handleLogin} className="product-form">
            <div className="form-group">
              <label htmlFor="login-username">Username</label>
              <input
                id="login-username"
                autoComplete="username"
                value={username}
                onChange={(event) => setUsername(event.target.value)}
                required
              />
            </div>
            <div className="form-group">
              <label htmlFor="login-password">Password</label>
              <input
                id="login-password"
                type="password"
                autoComplete="current-password"
                value={password}
                onChange={(event) => setPassword(event.target.value)}
                required
              />
            </div>
            <div className="form-actions-right">
              <button type="submit" className="btn-primary" disabled={loggingIn}>
                {loggingIn ? "Signing in..." : "Sign in"}
              </button>
            </div>
          </form>
        </section>
      </main>
    );
  }

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="brand">
          <div className="brand-badge">R</div>
          <div>
            <div className="brand-title">Retail Order &amp; Stock</div>
            <div className="brand-sub">Store Workspace</div>
          </div>
        </div>

        <nav className="nav-list">
          <button
            type="button"
            className={`nav-item ${activeScreen === "products" ? "active" : ""}`}
            onClick={() => setActiveScreen("products")}
          >
            Products
          </button>
          <button
            type="button"
            className={`nav-item ${activeScreen === "create-order" ? "active" : ""}`}
            onClick={() => setActiveScreen("create-order")}
          >
            Create Order
          </button>
          <button
            type="button"
            className={`nav-item ${activeScreen === "quote-summary" ? "active" : ""}`}
            onClick={() => setActiveScreen("quote-summary")}
          >
            Quote Summary
          </button>
          {role === "Admin" && (
            <button
              type="button"
              className={`nav-item ${activeScreen === "product-mgmt" ? "active" : ""}`}
              onClick={() => {
                setEditingProduct(null);
                setActiveScreen("product-mgmt");
              }}
            >
              Product Mgmt
            </button>
          )}
        </nav>

        <div className="sidebar-footer">
          <span className="role-dot" />
          <span>{role || "Store Staff / Admin"}</span>
          {!USE_MOCK_API && (
            <button type="button" className="btn-link" onClick={handleLogout}>
              Sign out
            </button>
          )}
        </div>
      </aside>

      <main className="main-content">
        {orderNotice && (
          <div className="success-banner" role="status" style={{ marginBottom: "1rem" }}>
            {orderNotice}
          </div>
        )}

        {activeScreen === "products" && (
          <Products
            products={products}
            loading={loadingProducts}
            error={productsError}
            onRefresh={loadProducts}
            onEditProduct={role === "Admin" ? handleSelectEditProduct : undefined}
            onCreateOrder={() => setActiveScreen("create-order")}
          />
        )}

        {activeScreen === "product-mgmt" && role === "Admin" && (
          <ProductManagement
            products={products}
            selectedProduct={editingProduct}
            onClearSelectedProduct={() => setEditingProduct(null)}
            onSaved={loadProducts}
            onCancel={() => {
              setEditingProduct(null);
              setActiveScreen("products");
            }}
          />
        )}

        {activeScreen === "create-order" && (
          <CreateOrder
            products={products}
            draftOrder={quoteDraft}
            onQuoteReady={handleQuoteReady}
            onCancel={() => setActiveScreen("products")}
          />
        )}

        {activeScreen === "quote-summary" && (
          <QuoteSummary
            quote={quoteDraft}
            onBack={() => setActiveScreen("create-order")}
            onOrderSubmitted={handleOrderSubmitted}
          />
        )}

        {submittedOrders.length > 0 && (
          <section className="panel recent-submissions">
            <h3>Submitted Orders This Session ({submittedOrders.length})</h3>
            <ul className="session-order-list">
              {submittedOrders.map((ord, idx) => (
                <li key={ord.orderId ?? idx} className="session-order-row">
                  <span>
                    <strong>{ord.customerName}</strong> —{" "}
                    <span className="status-pill status-active">{ord.status ?? "Pending"}</span>
                  </span>
                  <strong>R{Number(ord.finalTotal ?? 0).toFixed(2)}</strong>
                </li>
              ))}
            </ul>
          </section>
        )}
      </main>
    </div>
  );
}