# Technology Store - Mikroservis Mimarisi

## Proje Yapısı

### Backend (Mikroservisler)

- **ApiGateway**: Tüm istekleri yönlendiren gateway ✅ **Tamamlandı**
- **IdentityService**: Kullanıcı kimlik doğrulama ve yetkilendirme ✅ **Tamamlandı**
- **ProductService**: Ürün ve kategori yönetimi ✅ **Tamamlandı**
- **BasketService**: Sepet yönetimi ve Redis cache ✅ **Tamamlandı**
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

### 📋 Faz 4 - E-Ticaret Core - Tamamlandı ✅

- [x] **BasketService** - Redis Cache, Sepet Yönetimi (Port: 5002)
  - [x] CQRS with MediatR pattern
  - [x] Clean Architecture (Domain, Application, Infrastructure, API)
  - [x] Redis cache integration (StackExchange.Redis)
  - [x] RedisInsight UI entegrasyonu (Port: 5540)
  - [x] Basket management endpoints:
    - AddItemToBasket - Sepete ürün ekleme
    - GetBasket - Sepeti görüntüleme
    - UpdateItemQuantity - Ürün miktarı güncelleme
    - RemoveItemFromBasket - Ürün silme
    - ClearBasket - Sepeti temizleme
  - [x] FluentValidation with custom validators
  - [x] AutoMapper entity-DTO mapping
  - [x] Comprehensive logging system:
    - LoggingBehavior (MediatR pipeline)
    - Repository level logging
    - Controller endpoint logging
    - Startup/Shutdown banners
  - [x] Global exception handling middleware
  - [x] Serilog with structured logging
  - [x] Real-time data tracking via RedisInsight
  - [x] Gateway integration completed
- [ ] **OrderService** - Saga Pattern, Sipariş İşleme
- [ ] **PaymentService** - Ödeme Entegrasyonu

### 📋 Faz 5 - Destek Servisleri

- [ ] **NotificationService** - Event-Driven, Email/SMS

## Servis Port Yapısı

| Servis          | API Port | Database/Cache Port | UI Port |
| --------------- | -------- | ------------------- | ------- |
| ApiGateway      | 5050     | -                   | -       |
| ProductService  | 5000     | 1450 (SQL Server)   | -       |
| IdentityService | 5001     | 1450 (SQL Server)   | -       |
| BasketService   | 5002     | 6379 (Redis)        | 5540    |

## Swagger UI

- **API Gateway (Aggregated)**: http://localhost:5050/swagger/index.html ⭐ **Öneri: Buradan kullan!**
- **ProductService**: http://localhost:5000/swagger
- **IdentityService**: http://localhost:5001/swagger
- **BasketService**: http://localhost:5002/swagger

## Redis Yönetim Araçları

- **RedisInsight**: http://localhost:5540
  - Basket verilerini görsel olarak izleme
  - Key-value çiftlerini inceleme
  - Real-time data monitoring
  - Bağlantı ayarları: Host=`redis`, Port=`6379`

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

### Docker Servisleri

```bash
# Tüm altyapı servislerini başlat
docker-compose up -d

# Servisler:
# - SQL Server (Port: 1450)
# - Redis (Port: 6379)
# - RabbitMQ (Port: 5672, Management: 15672)
# - RedisInsight (Port: 5540)
```

### Backend

```bash
cd backend/src
dotnet restore

# Her servisi ayrı terminalde çalıştırın:
# ProductService (Port: 5000)
cd Services/ProductService/ProductService.API
dotnet run

# IdentityService (Port: 5001)
cd Services/IdentityService/IdentityService.API
dotnet run

# BasketService (Port: 5002)
cd Services/BasketService/BasketService.API
dotnet run

# API Gateway (Port: 5050) - En son başlatın
cd ApiGateway
dotnet run
```

### Frontend

```bash
cd frontend
npm install
npm run dev
```

## Teknolojiler

### Backend

- .NET 9.0
- Entity Framework Core
- MediatR (CQRS)
- FluentValidation
- AutoMapper
- Serilog
- xUnit & NSubstitute (Testing)
- Ocelot (API Gateway)
- Polly (Resilience & Circuit Breaker)

### Database & Cache

- SQL Server
- Redis
- RedisInsight

### Message Broker

- RabbitMQ
- MassTransit (hazırlık)

### DevOps

- Docker & Docker Compose

### Frontend

- Next.js 14
- TypeScript
- React
