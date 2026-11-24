import { useCallback, useEffect, useState } from "react";
import { getProducts, login, USE_MOCK_API } from "./services/api";
import Products from "./pages/Products";
import ProductManagement from "./pages/ProductManagement";
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
  const [products, setProducts] = useState([]);
  const [loadingProducts, setLoadingProducts] = useState(true);
  const [productsError, setProductsError] = useState("");
  const [activeScreen, setActiveScreen] = useState("products");
  const canManageProducts = ["admin", "manager"].includes(role.toLowerCase());

  const loadProducts = useCallback(async () => {
    if (!USE_MOCK_API && !sessionStorage.getItem("retailOrderAccessToken")) return;

    setLoadingProducts(true);
    setProductsError("");
    try {
      setProducts(await getProducts());
    } catch (err) {
      setProductsError(err.message || "Failed to load products.");
    } finally {
      setLoadingProducts(false);
    }
  }, []);

  useEffect(() => {
    if (!accessToken) return;

    let active = true;
    getProducts()
      .then((result) => {
        if (active) {
          setProducts(result);
          setProductsError("");
        }
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
      setLoadingProducts(true);
      setProductsError("");
      setAccessToken(result.token);
      setRole(result.role);
      setPassword("");
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
  }

  if (!USE_MOCK_API && !accessToken) {
    return (
      <main className="main-content">
        <section className="panel">
          <h1>Sign in</h1>
          <p className="subtitle">Sign in to use the store workspace.</p>
          {loginError && (
            <div className="error-banner" role="alert">
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

        <nav className="nav-list" aria-label="Main navigation">
          <button
            type="button"
            className={`nav-item ${activeScreen === "products" ? "active" : ""}`}
            onClick={() => setActiveScreen("products")}
          >
            Products
          </button>
          {canManageProducts && (
            <button
              type="button"
              className={`nav-item ${activeScreen === "product-management" ? "active" : ""}`}
              onClick={() => setActiveScreen("product-management")}
            >
              Product Management
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
        {activeScreen === "product-management" && canManageProducts ? (
          <ProductManagement products={products} onSaved={loadProducts} />
        ) : (
          <Products
            products={products}
            loading={loadingProducts}
            error={productsError}
            onRefresh={loadProducts}
          />
        )}
      </main>
    </div>
  );
}