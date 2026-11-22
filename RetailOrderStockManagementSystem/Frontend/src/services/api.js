const BASE_URL = (import.meta.env.VITE_API_BASE_URL ?? "").replace(/\/+$/, "");
export const USE_MOCK_API = import.meta.env.VITE_USE_MOCK_API === "true";

const MOCK_PRODUCTS = [
  { productId: 1, sku: "SKU-1001", name: "Wireless Mouse", category: "Accessories", unitPrice: 350.0, stockQuantity: 24, isActive: true },
  { productId: 2, sku: "SKU-1002", name: "HD Webcam", category: "Electronics", unitPrice: 890.0, stockQuantity: 12, isActive: true },
  { productId: 3, sku: "SKU-1003", name: "USB-C Hub", category: "Accessories", unitPrice: 520.0, stockQuantity: 18, isActive: true },
  { productId: 4, sku: "SKU-1004", name: "Laptop Stand", category: "Accessories", unitPrice: 450.0, stockQuantity: 4, isActive: true },
  { productId: 5, sku: "SKU-1005", name: "Phone Charger", category: "Accessories", unitPrice: 180.0, stockQuantity: 3, isActive: true },
  { productId: 6, sku: "SKU-1006", name: "Keyboard Cover", category: "Accessories", unitPrice: 95.0, stockQuantity: 1, isActive: true },
  { productId: 7, sku: "SKU-1007", name: "Bluetooth Speaker", category: "Electronics", unitPrice: 760.0, stockQuantity: 9, isActive: true },
  { productId: 8, sku: "SKU-1008", name: "Screen Cleaner Kit", category: "Accessories", unitPrice: 65.0, stockQuantity: 2, isActive: true },
  { productId: 9, sku: "SKU-1009", name: "Legacy VGA Adapter", category: "Electronics", unitPrice: 120.0, stockQuantity: 15, isActive: false },
];

function getAuthHeader() {
  const token = sessionStorage.getItem("retailOrderAccessToken");
  return token ? { Authorization: `Bearer ${token}` } : {};
}

async function request(path, options = {}) {
  const response = await fetch(`${BASE_URL}${path}`, {
    headers: {
      "Content-Type": "application/json",
      ...getAuthHeader(),
      ...(options.headers || {}),
    },
    ...options,
  });

  if (!response.ok) {
    let message = `Request failed (${response.status})`;
    try {
      const errorBody = await response.json();
      message = errorBody.message || errorBody.title || message;
    } catch {
      const text = await response.text();
      if (text) message = text;
    }
    throw new Error(message);
  }

  if (response.status === 204) return null;
  return response.json();
}

export async function getProducts(params = {}) {
  if (USE_MOCK_API) {
    return structuredClone(MOCK_PRODUCTS);
  }

  const searchParams = new URLSearchParams();
  if (params.search) searchParams.set("search", params.search);
  if (params.category && params.category !== "ALL") searchParams.set("category", params.category);

  const query = searchParams.toString();
  return request(`/api/products${query ? `?${query}` : ""}`);
}

export async function login(username, password) {
  return request("/api/Auth/Login", {
    method: "POST",
    body: JSON.stringify({ username, password }),
  });
}

export async function createProduct(payload) {
  if (USE_MOCK_API) {
    const duplicate = MOCK_PRODUCTS.some(
      (p) => p.sku.trim().toLowerCase() === payload.sku.trim().toLowerCase()
    );
    if (duplicate) {
      throw new Error(`SKU "${payload.sku}" already exists.`);
    }

    const nextId = Math.max(0, ...MOCK_PRODUCTS.map((p) => p.productId)) + 1;
    const created = {
      productId: nextId,
      sku: payload.sku.trim(),
      name: payload.name.trim(),
      category: payload.category.trim() || "General",
      unitPrice: Number(payload.unitPrice),
      stockQuantity: Number(payload.stockQuantity),
      isActive: Boolean(payload.isActive),
    };
    MOCK_PRODUCTS.push(created);
    return structuredClone(created);
  }

  return request("/api/products", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function updateProduct(id, payload) {
  if (USE_MOCK_API) {
    const idx = MOCK_PRODUCTS.findIndex((p) => p.productId === id);
    if (idx === -1) throw new Error("Product not found.");

    const duplicate = MOCK_PRODUCTS.some(
      (p) =>
        p.productId !== id &&
        p.sku.trim().toLowerCase() === payload.sku.trim().toLowerCase()
    );
    if (duplicate) {
      throw new Error(`SKU "${payload.sku}" already exists.`);
    }

    MOCK_PRODUCTS[idx] = {
      ...MOCK_PRODUCTS[idx],
      sku: payload.sku.trim(),
      name: payload.name.trim(),
      category: payload.category.trim() || "General",
      unitPrice: Number(payload.unitPrice),
      stockQuantity: Number(payload.stockQuantity),
      isActive: Boolean(payload.isActive),
      productId: id,
    };
    return structuredClone(MOCK_PRODUCTS[idx]);
  }

  return request(`/api/products/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  });
}

const MOCK_CUSTOMERS = [
  { customerId: 1, fullName: "Thabo Mokoena", mobileNumber: "082 123 4567", email: "thabo@example.com", isActive: true },
  { customerId: 2, fullName: "Aisha Patel", mobileNumber: "083 234 5678", email: "aisha@example.com", isActive: true },
  { customerId: 3, fullName: "Sipho Dlamini", mobileNumber: "084 345 6789", email: "sipho@example.com", isActive: true },
  { customerId: 4, fullName: "Lerato Nkosi", mobileNumber: "071 456 7890", email: "lerato@example.com", isActive: true },
];

const MOCK_ORDERS = [];

function roundMoney(value) {
  return Math.round((Number(value) + Number.EPSILON) * 100) / 100;
}

export async function getCustomers() {
  if (USE_MOCK_API) {
    return structuredClone(MOCK_CUSTOMERS.filter((c) => c.isActive));
  }
  return request("/api/customers");
}

export async function requestOrderQuote(payload) {
  if (USE_MOCK_API) {
    const customer = MOCK_CUSTOMERS.find((c) => c.customerId === Number(payload.customerId));
    if (!customer) {
      throw new Error("Please select a valid customer.");
    }

    if (!Array.isArray(payload.items) || payload.items.length === 0) {
      throw new Error("An order must contain at least one line item.");
    }

    const lines = [];
    for (const row of payload.items) {
      const product = MOCK_PRODUCTS.find((p) => p.productId === Number(row.productId));
      const qty = Number(row.quantity);

      if (!product) {
        throw new Error("Selected product does not exist.");
      }
      if (!product.isActive) {
        throw new Error(`Product "${product.name}" is inactive and cannot be ordered.`);
      }
      if (!Number.isInteger(qty) || qty < 1) {
        throw new Error(`Quantity for "${product.name}" must be a positive whole number.`);
      }
      if (qty > product.stockQuantity) {
        throw new Error(
          `Requested quantity (${qty}) for "${product.name}" exceeds available stock (${product.stockQuantity}).`
        );
      }

      const unitPrice = roundMoney(product.unitPrice);
      const lineTotal = roundMoney(unitPrice * qty);

      lines.push({
        productId: product.productId,
        sku: product.sku,
        productName: product.name,
        quantity: qty,
        unitPrice,
        lineTotal,
      });
    }

    const subtotal = roundMoney(lines.reduce((sum, l) => sum + l.lineTotal, 0));
    const qualifiesForDiscount = subtotal >= 2000;
    const discountPercent = qualifiesForDiscount ? 10 : 0;
    const discountAmount = roundMoney(subtotal * (discountPercent / 100));
    const finalTotal = Math.max(0, roundMoney(subtotal - discountAmount));

    return {
      customerId: customer.customerId,
      customerName: customer.fullName,
      items: payload.items.map((row) => ({
        productId: Number(row.productId),
        quantity: Number(row.quantity),
      })),
      lines,
      subtotal,
      discountPercent,
      discountAmount,
      finalTotal,
    };
  }

  return request("/api/orders/quote", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function createOrder(payload) {
  if (USE_MOCK_API) {
    const quote = await requestOrderQuote(payload);
    const createdOrder = {
      orderId: `ORD-${Date.now()}`,
      customerId: quote.customerId,
      customerName: quote.customerName,
      orderDate: new Date().toISOString(),
      subtotal: quote.subtotal,
      discountPercent: quote.discountPercent,
      discountAmount: quote.discountAmount,
      finalTotal: quote.finalTotal,
      status: "Pending",
      lines: quote.lines,
    };

    MOCK_ORDERS.unshift(createdOrder);
    return structuredClone(createdOrder);
  }

  return request("/api/orders", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}