// ==================== AUTH TYPES ====================
export interface User {
  id: string;
  email: string;
  firstName?: string;
  lastName?: string;
  roles?: string[];
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}

export interface AuthResponse {
  token?: string;
  accessToken?: string;
  refreshToken?: string;
  expiresAt?: string;
  userId?: string;
  id?: string;
  user?: User;
}

// ==================== PRODUCT TYPES ====================
export interface Product {
  id: string;
  name: string;
  description?: string;
  price: number;
  stock?: number;
  category?: string;
  categoryId?: string;
  categoryName?: string;
  brand?: string;
  imageUrl?: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface Category {
  id: string;
  name: string;
  description?: string;
  parentId?: string;
}

// ==================== BASKET TYPES ====================
export interface BasketItem {
  productId: string;
  productName: string;
  price: number;
  quantity: number;
  imageUrl?: string;
}

export interface Basket {
  userId: string;
  items: BasketItem[];
  totalPrice?: number;
}

export interface AddToBasketRequest {
  productId: string;
  productName: string;
  price: number;
  quantity: number;
}

// ==================== ORDER TYPES ====================
export enum OrderStatus {
  Pending = "Pending",
  PaymentReceived = "PaymentReceived",
  Processing = "Processing",
  Shipped = "Shipped",
  Delivered = "Delivered",
  Cancelled = "Cancelled",
  Failed = "Failed",
}

export interface OrderItem {
  productId: string;
  productName: string;
  quantity: number;
  price: number;
  subtotal?: number;
}

export interface ShippingAddress {
  street: string;
  city: string;
  state: string;
  country: string;
  zipCode: string;
}

export interface Order {
  id: string;
  userId: string;
  status: OrderStatus | string | number;
  items: OrderItem[];
  totalAmount?: number;
  shippingAddress?: ShippingAddress;
  paymentMethod?: string;
  paymentIntentId?: string;
  notes?: string;
  createdDate?: string;
  updatedDate?: string;
  completedDate?: string;
  cancelledDate?: string;
  cancellationReason?: string;
}

export interface CreateOrderRequest {
  userId: string;
  items: OrderItem[];
  shippingAddress: ShippingAddress;
  paymentMethod?: string;
  notes?: string;
}

// ==================== PAYMENT TYPES ====================
export enum PaymentStatus {
  Pending = "Pending",
  Processing = "Processing",
  Success = "Success",
  Failed = "Failed",
  Refunded = "Refunded",
}

export interface Payment {
  id: string;
  orderId: string;
  userId: string;
  amount: number;
  currency: string;
  status: PaymentStatus | string;
  transactionId?: string;
  failureReason?: string;
  processedDate?: string;
  createdDate?: string;
}

export interface CreditCard {
  id: string;
  cardHolderName: string;
  cardNumber: string;
  expiryMonth: string | number;
  expiryYear: string | number;
  cardType?: string;
  isDefault?: boolean;
  createdDate?: string;
}

// ==================== API RESPONSE TYPES ====================
export interface ApiError {
  title?: string;
  status?: number;
  errors?: Record<string, string[]>;
  message?: string;
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

// ==================== UI TYPES ====================
export interface StatusMeta {
  label: string;
  badge: string;
  description: string;
}

export interface ToastMessage {
  id: string;
  type: "success" | "error" | "warning" | "info";
  message: string;
  duration?: number;
}
