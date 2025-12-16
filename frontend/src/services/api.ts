
import {
  Product,
  Category,
  Basket,
  Order,
  User,
  LoginRequest,
  AuthResponse,
  RegisterRequest,
  Review,
  Payment,
  AddToBasketRequest,
  CreateOrderRequest,
  CreditCard
} from "@/types";

// =======================
// STATIC DUMMY DATA (RANDOMIZED CONTENT)
// =======================

const CATEGORIES: Category[] = [
  { id: "c-gaming", name: "Gaming", description: "Yüksek performanslı oyuncu ekipmanları" },
  { id: "c-phone", name: "Telefon", description: "Son teknoloji akıllı telefonlar" },
  { id: "c-computer", name: "Bilgisayar", description: "Laptop ve Masaüstü bilgisayarlar" },
  { id: "c-audio", name: "Ses Sistemleri", description: "Kulaklık ve Hoparlörler" },
  { id: "c-wearable", name: "Giyilebilir Teknoloji", description: "Akıllı saatler ve bileklikler" },
  { id: "c-camera", name: "Kamera", description: "Profesyonel fotoğraf ve video ekipmanları" },
];

const PRODUCTS: Product[] = [
  {
    id: "p-1",
    name: "Mustafa Esen",
    description: "Mustafa Esen büyük bir kaşar",
    price: 1,
    stock: 1,
    category: "Kaşar",
    categoryId: "c-gaming",
    brand: "Nebula",
    imageUrl: "https://images.unsplash.com/photo-1603302576837-37561b2e2302?auto=format&fit=crop&q=80&w=1000",
  },
  {
    id: "p-2",
    name: "Egemen Saygın",
    description: "2 numaralı kaşar. Kaşarı kaşırmak için idealdir.",
    price: 31,
    stock: 1,
    category: "Kaşar",
    categoryId: "c-phone",
    brand: "Spectra",
    imageUrl: "https://images.unsplash.com/photo-1556656793-02715d8dd660?auto=format&fit=crop&q=80&w=1000",
  },
  {
    id: "p-3",
    name: "Seymen Kocatürk",
    description: "insanları 5 saat bekler salaklık da bi yere kadar",
    price: 69,
    stock: 1,
    category: "enayi",
    categoryId: "c-audio",
    brand: "Sonic",
    imageUrl: "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?auto=format&fit=crop&q=80&w=1000",
  },
  {
    id: "p-4",
    name: "Orospu Mustafa",
    description: "Kaşar Mustafanın orospu hali 5 para etmez",
    price: 0,
    stock: 1,
    category: "Genel Verici",
    categoryId: "c-wearable",
    brand: "Titanium",
    imageUrl: "https://images.unsplash.com/photo-1523275335684-37898b6baf30?auto=format&fit=crop&q=80&w=1000",
  },
];

const mockUser: User = {
  id: "u-demo",
  email: "kaşarmusti@gmail.com",
  firstName: "Kaşar",
  lastName: "Musti",
  roles: ["Customer", "Admin"],
};

// In-memory state for the session
let currentBasket: Basket = {
  userId: "u-demo",
  items: [],
  totalPrice: 0,
};

const mockAddress = {
  street: "Teknoloji Mah. Yazılım Sok. No:1",
  city: "Istanbul",
  state: "TR",
  country: "Turkey",
  zipCode: "34000"
};

let mockOrders: Order[] = [
  {
    id: "order-1",
    userId: "u-demo",
    items: [
      { productId: "p-1", productName: "Nebula X1 Gaming Laptop", price: 89999, quantity: 1 }
    ],
    totalAmount: 89999,
    status: "Delivered",
    paymentMethod: "Credit Card",
    shippingAddress: mockAddress,
    createdDate: new Date(Date.now() - 86400000 * 5).toISOString()
  },
  {
    id: "order-2",
    userId: "u-demo",
    items: [
      { productId: "p-3", productName: "SonicBlast Pro", price: 8500, quantity: 2 }
    ],
    totalAmount: 17000,
    status: "Shipped",
    paymentMethod: "Credit Card",
    shippingAddress: mockAddress,
    createdDate: new Date(Date.now() - 86400000 * 2).toISOString()
  }
];

const delay = (ms = 300) => new Promise((resolve) => setTimeout(resolve, ms));

// =======================
// MOCK SERVICES (Replacing Real API)
// =======================

export const ProductService = {
  getAll: async (): Promise<Product[]> => {
    await delay();
    return [...PRODUCTS];
  },
  getById: async (id: string): Promise<Product | undefined> => {
    await delay();
    return PRODUCTS.find(p => p.id === id);
  },
  create: async (product: any) => { await delay(); return product; },
  update: async (id: string, product: any) => { await delay(); return product; },
  delete: async (id: string) => { await delay(); },
};

export const CategoryService = {
  getAll: async (): Promise<Category[]> => {
    await delay();
    return [...CATEGORIES];
  },
  create: async (cat: any) => { await delay(); return cat; },
  update: async (id: string, cat: any) => { await delay(); return cat; },
  delete: async (id: string) => { await delay(); },
};

export const AuthService = {
  login: async (creds: LoginRequest): Promise<AuthResponse> => {
    await delay(600);
    return {
      token: "mock-jwt-token-xyz-123",
      refreshToken: "mock-refresh-token",
      userId: mockUser.id,
      user: mockUser,
    };
  },
  register: async (creds: RegisterRequest): Promise<AuthResponse> => {
    await delay(600);
    return {
      token: "mock-jwt-token-xyz-123",
      refreshToken: "mock-refresh-token",
      userId: mockUser.id,
      user: mockUser,
    };
  },
  getCurrentUser: async (): Promise<User> => {
    await delay();
    return mockUser;
  },
  refreshToken: async () => {
    return { token: "refreshed-mock-token" };
  }
};

export const BasketService = {
  getBasket: async (userId: string): Promise<Basket> => {
    await delay(200);
    return { ...currentBasket };
  },
  addItem: async (userId: string, item: AddToBasketRequest): Promise<Basket> => {
    await delay(200);
    const existing = currentBasket.items.find(i => i.productId === item.productId);
    if (existing) {
      existing.quantity += item.quantity || 1;
    } else {
      // Find product image for better UX
      const prod = PRODUCTS.find(p => p.id === item.productId);
      currentBasket.items.push({
        productId: item.productId,
        productName: item.productName || "Unknown",
        price: item.price || 0,
        quantity: item.quantity || 1,
        imageUrl: prod?.imageUrl
      });
    }
    // Recalculate total
    currentBasket.totalPrice = currentBasket.items.reduce((acc, i) => acc + (i.price * i.quantity), 0);
    return { ...currentBasket };
  },
  updateItem: async (userId: string, productId: string, quantity: number): Promise<Basket> => {
    await delay(200);
    const existing = currentBasket.items.find(i => i.productId === productId);
    if (existing) {
      existing.quantity = quantity;
    }
    currentBasket.totalPrice = currentBasket.items.reduce((acc, i) => acc + (i.price * i.quantity), 0);
    return { ...currentBasket };
  },
  removeItem: async (userId: string, productId: string): Promise<void> => {
    await delay(200);
    currentBasket.items = currentBasket.items.filter(i => i.productId !== productId);
    currentBasket.totalPrice = currentBasket.items.reduce((acc, i) => acc + (i.price * i.quantity), 0);
  },
  clear: async (userId: string): Promise<void> => {
    await delay(200);
    currentBasket.items = [];
    currentBasket.totalPrice = 0;
  }
};

export const OrderService = {
  createOrder: async (payload: CreateOrderRequest): Promise<Order> => {
    await delay(800);
    const newOrder: Order = {
      id: `ord-${Math.floor(Math.random() * 10000)}`,
      userId: payload.userId,
      items: payload.items,
      totalAmount: payload.items.reduce((acc, i) => acc + (i.price * i.quantity), 0),
      status: "Pending",
      paymentMethod: payload.paymentMethod,
      shippingAddress: payload.shippingAddress,
      createdDate: new Date().toISOString()
    };
    mockOrders.unshift(newOrder);
    // Clear basket after order
    currentBasket.items = [];
    currentBasket.totalPrice = 0;
    return newOrder;
  },
  getAllOrders: async (): Promise<Order[]> => {
    await delay();
    return [...mockOrders];
  },
  getUserOrders: async (userId: string): Promise<Order[]> => {
    await delay();
    return [...mockOrders]; // Return all for demo
  },
  getOrder: async (id: string) => {
    await delay();
    return mockOrders.find(o => o.id === id);
  },
  cancelOrder: async (id: string, reason: string) => {
    await delay();
    const o = mockOrders.find(o => o.id === id);
    if (o) o.status = "Cancelled";
    return o;
  }
};

export const UserService = {
  getAllUsers: async (): Promise<User[]> => {
    await delay();
    return [
      mockUser,
      { id: "u-2", email: "ali@veli.com", firstName: "Ali", lastName: "Veli", roles: ["Customer"] },
      { id: "u-3", email: "ayse@fatma.com", firstName: "Ayşe", lastName: "Fatma", roles: ["Customer"] }
    ];
  },
  updateUser: async () => { },
  deleteUser: async () => { }
};

export const PaymentService = {
  getAllPayments: async () => { await delay(); return []; },
  getPaymentsByUserId: async () => { await delay(); return []; }
};

export const ReviewService = {
  getByProduct: async (pid: string): Promise<Review[]> => {
    await delay();
    return [];
  },
  getAll: async (): Promise<Review[]> => {
    await delay();
    return [];
  },
  create: async (data: any): Promise<Review> => {
    await delay();
    return {
      id: "rev-" + Math.random(),
      createdAt: new Date().toISOString(),
      ...data
    };
  },
  delete: async () => { }
};

export const CreditCardService = {
  getDefault: async (): Promise<CreditCard | null> => {
    await delay();
    return {
      id: "cc-1",
      cardHolderName: "Demo User",
      cardNumber: "**** **** **** 1234",
      expiryMonth: 12,
      expiryYear: 2028,
      isDefault: true
    };
  },
  create: async (data: any): Promise<CreditCard> => {
    await delay();
    return {
      id: "cc-" + Math.random(),
      ...data
    };
  }
};
