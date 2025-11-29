import axios from "axios";

// Tüm istekler API Gateway üzerinden geçsin
const GATEWAY_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5050/api";
const fallbackBrands = ["Apple", "Samsung", "Sony", "Dell", "HP"];

export const api = axios.create({
  baseURL: GATEWAY_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

// Token eklemek için interceptor
api.interceptors.request.use((config) => {
  if (typeof window !== "undefined") {
    const token = localStorage.getItem("token");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
  }
  return config;
});

export const ProductService = {
  getAll: async () => {
    const response = await api.get("/products");
    const products = Array.isArray(response.data) ? response.data : [];
    return products.map((p: any, idx: number) => ({
      ...p,
      id: (p.id ?? idx).toString(),
      price: Number(p.price ?? 0),
      category: p.category ?? p.categoryName ?? "General",
      brand: p.brand ?? fallbackBrands[idx % fallbackBrands.length],
    }));
  },
  getById: async (id: string) => {
    const response = await api.get(`/products/${id}`);
    const p = response.data;
    return {
      ...p,
      id: (p?.id ?? id).toString(),
      price: Number(p?.price ?? 0),
      category: p?.category ?? p?.categoryName ?? "General",
      brand: p?.brand ?? fallbackBrands[0],
    };
  },
};

export const AuthService = {
  login: async (payload: { email: string; password: string }) => {
    const res = await api.post("/auth/login", payload);
    return res.data;
  },
  register: async (payload: { firstName: string; lastName: string; email: string; password: string }) => {
    const res = await api.post("/auth/register", payload);
    return res.data;
  },
};

export const BasketService = {
  getBasket: async (userId: string) => {
    const res = await api.get(`/baskets/${userId}`);
    return res.data;
  },
  addItem: async (
    userId: string,
    item: { productId: string; productName: string; price: number; quantity: number }
  ) => {
    const res = await api.post(`/baskets/${userId}/items`, item);
    return res.data;
  },
  updateItem: async (userId: string, productId: string, quantity: number) => {
    const res = await api.put(`/baskets/${userId}/items/${productId}`, { quantity });
    return res.data;
  },
  removeItem: async (userId: string, productId: string) => {
    const res = await api.delete(`/baskets/${userId}/items/${productId}`);
    return res.data;
  },
  clear: async (userId: string) => {
    const res = await api.delete(`/baskets/${userId}`);
    return res.data;
  },
};

export const OrderService = {
  createOrder: async (payload: {
    userId: string;
    items: { productId: string; quantity: number; price: number }[];
    address: { street: string; city: string; country: string; zipCode: string };
    paymentMethod?: string;
  }) => {
    const res = await api.post("/orders", payload);
    return res.data;
  },
  getOrder: async (orderId: string) => {
    const res = await api.get(`/orders/${orderId}`);
    return res.data;
  },
  getUserOrders: async (userId: string) => {
    const res = await api.get(`/orders/user/${userId}`);
    return res.data;
  },
};
