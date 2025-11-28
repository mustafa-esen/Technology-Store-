# Technology Store - Mikroservis Mimarisi

## Proje Yapısı

### Backend (Mikroservisler)

- **ApiGateway**: Tüm istekleri yönlendiren gateway
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

### 📋 Faz 3 - API Gateway ve İletişim

- [ ] **ApiGateway** - Ocelot/YARP, JWT Validation, Rate Limiting

### 📋 Faz 4 - E-Ticaret Core

- [ ] **BasketService** - Redis Cache, Sepet Yönetimi
- [ ] **OrderService** - Saga Pattern, Sipariş İşleme
- [ ] **PaymentService** - Ödeme Entegrasyonu

### 📋 Faz 5 - Destek Servisleri

- [ ] **NotificationService** - Event-Driven, Email/SMS

## Servis Port Yapısı

| Servis          | API Port | Database Port |
| --------------- | -------- | ------------- |
| ProductService  | 5000     | 1450          |
| IdentityService | 5001     | 1450          |

## Swagger UI

- **ProductService**: http://localhost:5000
- **IdentityService**: http://localhost:5001

### Frontend

- **Next.js** with TypeScript
- **React** components
- **Tailwind CSS** (isteğe bağlı)

### Infrastructure

- **Docker** containers
- **Kubernetes** orchestration
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
