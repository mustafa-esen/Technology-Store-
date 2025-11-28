import axios from "axios";

// Environment variables can be used here
const API_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000/api"; // Product Service
const IDENTITY_URL = process.env.NEXT_PUBLIC_IDENTITY_URL || "http://localhost:5001/api"; // Identity Service
const fallbackBrands = ["Apple", "Samsung", "Sony", "Dell", "HP"];

export const api = axios.create({
    baseURL: API_URL,
    headers: {
        "Content-Type": "application/json",
    },
});

export const identityApi = axios.create({
    baseURL: IDENTITY_URL,
    headers: {
        "Content-Type": "application/json",
    },
});

// Add interceptor to attach token
identityApi.interceptors.request.use((config) => {
    const token = localStorage.getItem("token");
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

api.interceptors.request.use((config) => {
    const token = localStorage.getItem("token");
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
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
        const res = await identityApi.post("/auth/login", payload);
        return res.data;
    },
    register: async (payload: { name: string; email: string; password: string }) => {
        const res = await identityApi.post("/auth/register", payload);
        return res.data;
    },
};
