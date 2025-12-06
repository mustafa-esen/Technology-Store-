import axios, { InternalAxiosRequestConfig } from "axios";
import { getToken, isTokenExpired, clearAuthData } from "@/lib/auth";
import {
  Product,
  Category,
  Basket,
  AddToBasketRequest,
  Order,
  CreateOrderRequest,
  Payment,
  LoginRequest,
  RegisterRequest,
  AuthResponse,
} from "@/types";

// Tüm istekler API Gateway üzerinden geçsin
const GATEWAY_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5050/api";
const fallbackBrands = ["Apple", "Samsung", "Sony", "Dell", "HP"];

export const api = axios.create({
  baseURL: GATEWAY_URL,
  headers: {
    "Content-Type": "application/json",
  },
  timeout: 30000, // 30 saniye timeout (önceden 15 saniyeydi)
});

/**
 * Request interceptor - Auth token ekler
 */
const attachAuth = (config: InternalAxiosRequestConfig): InternalAxiosRequestConfig => {
  if (typeof window !== "undefined") {
    const token = getToken();
    if (token) {
      // Token expire olmuşsa temizle ve devam et
      if (isTokenExpired(token)) {
        clearAuthData();
      } else {
        config.headers.Authorization = `Bearer ${token}`;
      }
    }
  }
  return config;
};

/**
 * Response interceptor - 401 hatalarını yakalar
 */
const handleResponseError = (error: any) => {
  if (error.response?.status === 401) {
    // Token geçersiz, temizle
    if (typeof window !== "undefined") {
      clearAuthData();
      // Login sayfasına yönlendir (sadece browser'da)
      const currentPath = window.location.pathname;
      if (currentPath !== "/login" && currentPath !== "/register") {
        window.location.href = "/login";
      }
    }
  }
  return Promise.reject(error);
};

api.interceptors.request.use(attachAuth, (error) => Promise.reject(error));
api.interceptors.response.use((response) => response, handleResponseError);

// ==================== PRODUCT SERVICE ====================
export const ProductService = {
  getAll: async (): Promise<Product[]> => {
    const response = await api.get("/products");
    const products = Array.isArray(response.data) ? response.data : [];
    return products.map((p: any, idx: number) => ({
      ...p,
      id: (p.id ?? idx).toString(),
      price: Number(p.price ?? 0),
      category: p.category ?? p.categoryName ?? "General",
      categoryId: p.categoryId ?? p.category?.id ?? p.categoryID,
      brand: p.brand ?? fallbackBrands[idx % fallbackBrands.length],
      imageUrl: p.imageUrl,
    }));
  },

  getById: async (id: string): Promise<Product> => {
    const response = await api.get(`/products/${id}`);
    const p = response.data;
    return {
      ...p,
      id: (p?.id ?? id).toString(),
      price: Number(p?.price ?? 0),
      category: p?.category ?? p?.categoryName ?? "General",
      categoryId: p?.categoryId ?? p?.category?.id ?? p?.categoryID,
      brand: p?.brand ?? fallbackBrands[0],
      imageUrl: p?.imageUrl,
    };
  },

  create: async (payload: Partial<Product>): Promise<Product> => {
    const res = await api.post("/products", payload);
    return res.data;
  },

  update: async (id: string, payload: Partial<Product>): Promise<Product> => {
    const res = await api.put(`/products/${id}`, payload);
    return res.data;
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/products/${id}`);
  },
};

// ==================== CATEGORY SERVICE ====================
export const CategoryService = {
  getAll: async (): Promise<Category[]> => {
    const res = await api.get("/categories");
    return Array.isArray(res.data) ? res.data : [];
  },

  create: async (payload: Partial<Category>): Promise<Category> => {
    const res = await api.post("/categories", payload);
    return res.data;
  },

  update: async (id: string, payload: Partial<Category>): Promise<Category> => {
    const res = await api.put(`/categories/${id}`, payload);
    return res.data;
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/categories/${id}`);
  },
};

// ==================== AUTH SERVICE ====================
export const AuthService = {
  login: async (payload: LoginRequest): Promise<AuthResponse> => {
    const res = await api.post("/auth/login", payload);
    return res.data;
  },

  register: async (payload: RegisterRequest): Promise<AuthResponse> => {
    const res = await api.post("/auth/register", payload);
    return res.data;
  },

  refreshToken: async (refreshToken: string): Promise<AuthResponse> => {
    const res = await api.post("/auth/refresh-token", { refreshToken });
    return res.data;
  },

  getCurrentUser: async () => {
    const res = await api.get("/auth/me");
    return res.data;
  },
};

// ==================== BASKET SERVICE ====================
export const BasketService = {
  getBasket: async (userId: string): Promise<Basket> => {
    const res = await api.get(`/baskets/${userId}`);
    return res.data;
  },

  addItem: async (userId: string, item: AddToBasketRequest): Promise<Basket> => {
    const res = await api.post(`/baskets/${userId}/items`, item);
    return res.data;
  },

  updateItem: async (userId: string, productId: string, quantity: number): Promise<Basket> => {
    const res = await api.put(`/baskets/${userId}/items/${productId}`, { 
      userId,
      productId,
      quantity 
    });
    return res.data;
  },

  removeItem: async (userId: string, productId: string): Promise<void> => {
    await api.delete(`/baskets/${userId}/items/${productId}`);
  },

  clear: async (userId: string): Promise<void> => {
    await api.delete(`/baskets/${userId}`);
  },
};

// ==================== ORDER SERVICE ====================
export const OrderService = {
  createOrder: async (payload: CreateOrderRequest): Promise<Order> => {
    const res = await api.post("/orders", payload);
    return res.data;
  },

  getOrder: async (orderId: string): Promise<Order> => {
    const res = await api.get(`/orders/${orderId}`);
    return res.data;
  },

  getUserOrders: async (userId: string): Promise<Order[]> => {
    const res = await api.get(`/orders/user/${userId}`);
    return Array.isArray(res.data) ? res.data : [];
  },

  cancelOrder: async (orderId: string, reason: string): Promise<Order> => {
    const res = await api.put(`/orders/${orderId}/cancel`, { reason });
    return res.data;
  },
};

// ==================== PAYMENT SERVICE ====================
// Tüm istekler artık Gateway üzerinden geçiyor
export const PaymentService = {
  getPaymentById: async (id: string): Promise<Payment> => {
    const res = await api.get(`/payments/${id}`);
    return res.data;
  },

  getPaymentsByUserId: async (userId: string): Promise<Payment[]> => {
    const res = await api.get(`/payments/user/${userId}`);
    return Array.isArray(res.data) ? res.data : [];
  },
};
