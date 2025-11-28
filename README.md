# Technology Store - Mikroservis Mimarisi

## Proje Yapısı

### Backend (Mikroservisler)

- **ApiGateway**: Tüm istekleri yönlendiren gateway ✅ **Tamamlandı**
- **IdentityService**: Kullanıcı kimlik doğrulama ve yetkilendirme ✅ **Tamamlandı**
- **ProductService**: Ürün ve kategori yönetimi ✅ **Tamamlandı**
- **BasketService**: Sepet yönetimi ve Redis cache
- **OrderService**: Sipariş yönetimi
- **PaymentService**: Ödeme işlemleri
- **NotificationService**: E-posta ve SMS bildirimleri
- **Shared**: Ortak kütüphaneler ve modeller

## Geliştirme Sırası

### ✅ Faz 1 - Tamamlandı

- [x] **ProductService** - CQRS, Clean Architecture, 47 Unit Test (Port: 5000)

### ✅ Faz 2 - Kimlik Doğrulama - Tamamlandı

- [x] **IdentityService** - JWT Authentication, Role-based Authorization (Port: 5001)
  - [x] Register, Login, RefreshToken endpoints
  - [x] BCrypt password hashing
  - [x] JWT token generation & validation
  - [x] User roles: Admin, Customer
  - [x] Clean Architecture (Domain, Application, Infrastructure, API)
  - [x] CQRS with MediatR
  - [x] FluentValidation
  - [x] Serilog logging
  - [x] Global exception handling
  - [x] 63 Unit Tests (100% pass)

### ✅ Faz 3 - API Gateway ve İletişim - Tamamlandı

- [x] **ApiGateway** - Ocelot, Polly, Swagger Aggregation (Port: 5050)
  - [x] Route yönetimi ve yönlendirme
  - [x] Polly Circuit Breaker (3 hata → 10s break)
  - [x] Polly Timeout (5 saniye)
  - [x] Rate Limiting (100 istek/dakika)
  - [x] SwaggerForOcelot (Tüm servislerin tek UI'da toplanması)
  - [x] CORS yapılandırması
  - [x] Serilog logging
  - [x] Catch-all routes ({everything})
  - [x] 3 mikroservis entegrasyonu (Product, Category, Identity)

### 📋 Faz 4 - E-Ticaret Core

- [ ] **BasketService** - Redis Cache, Sepet Yönetimi
- [ ] **OrderService** - Saga Pattern, Sipariş İşleme
- [ ] **PaymentService** - Ödeme Entegrasyonu

### 📋 Faz 5 - Destek Servisleri

- [ ] **NotificationService** - Event-Driven, Email/SMS

## Servis Port Yapısı

| Servis          | API Port | Database Port |
| --------------- | -------- | ------------- |
| ApiGateway      | 5050     | -             |
| ProductService  | 5000     | 1450          |
| IdentityService | 5001     | 1450          |

## Swagger UI

- **API Gateway (Aggregated)**: http://localhost:5050/swagger/index.html ⭐ **Öneri: Buradan kullan!**
- **ProductService**: http://localhost:5000
- **IdentityService**: http://localhost:5001

### Frontend

- **Next.js** with TypeScript
- **React** components
- **Tailwind CSS** (isteğe bağlı)

### Infrastructure

- **Docker** containers
- **RabbitMQ** message broker
- **Redis** cache
- **PostgreSQL/SQL Server** database

## Başlangıç

### Backend

```bash
cd backend/src
dotnet restore
```

### Frontend

```bash
cd frontend
npm install
npm run dev
```

## Teknolojiler

- .NET 8.0
- Next.js 14
- TypeScript
- Docker
- RabbitMQ
- Redis
- Entity Framework Core

## Frontend Yol Haritası (Backend ile paralel)
- Faz 3 uyumu: Mevcut landing + ürün listesi (ProductService API), mock ürün detayı, login/register mock, cart client-state.
- Faz 4 uyumu: Sepet ekranını BasketService’e bağla, checkout/ödeme akışını PaymentService ile bağla, sipariş geçmişi ve detayı OrderService’ten oku, kimlik token’ını tüm çağrılara ekle.
- Faz 5 uyumu: NotificationService ile sipariş/kampanya bildirimlerini UI’da (banner/toast) göster, e-posta/SMS durumlarını yansıt.

## Frontend Durumu (ek not)
- Hazır sayfalar: Home (landing), Products listesi (ProductService API'den `/api/products`; hata olursa mock veri), Product detail (mock), Login/Register (mock), Cart (client-state).
- Env değişkenleri: `NEXT_PUBLIC_API_URL` (varsayılan `http://localhost:5000/api`), `NEXT_PUBLIC_IDENTITY_URL` (varsayılan `http://localhost:5001/api`).
- Backend DB şu an ProductService için InMemory; migration ve gerçek connection string ile kalıcı DB'ye geçildiğinde frontend çağrıları aynı kalır.
