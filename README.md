# Technology Store - Mikroservis Mimarisi

## Proje Yapısı

### Backend (Mikroservisler)

- **ApiGateway**: Tüm istekleri yönlendiren gateway ✅ **Tamamlandı**
- **IdentityService**: Kullanıcı kimlik doğrulama ve yetkilendirme ✅ **Tamamlandı**
- **ProductService**: Ürün ve kategori yönetimi ✅ **Tamamlandı**
- **BasketService**: Sepet yönetimi ve Redis cache ✅ **Tamamlandı**
- **OrderService**: Sipariş yönetimi ✅ **Tamamlandı**
- **PaymentService**: Ödeme işlemleri
- **NotificationService**: E-posta ve SMS bildirimleri
- **Shared**: Ortak kütüphaneler, event interface'leri ve RabbitMQ sabitleri ✅ **Tamamlandı**

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
  - [x] Docker containerization with multi-stage builds
  - [x] Emoji logging system with startup/shutdown banners

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
  - [x] Docker containerization
  - [x] Depends on all microservices (starts last)

### ✅ Faz 4 - E-Ticaret Core - Tamamlandı

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
    - **CheckoutBasket** - Sepet onaylama ve event yayınlama 🆕
  - [x] FluentValidation with custom validators
  - [x] AutoMapper entity-DTO mapping
  - [x] **Event-Driven Architecture:** 🆕
    - MassTransit 8.5.6 + RabbitMQ integration
    - IBasketCheckoutEvent publishing (anonymous type pattern)
    - Event içeriği: UserId, UserName, TotalPrice, ShippingAddress, Items
    - CheckoutBasket endpoint sepeti onaylar ve event yayınlar
  - [x] Comprehensive logging system:
    - LoggingBehavior (MediatR pipeline)
    - Repository level logging
    - Controller endpoint logging
    - Startup/Shutdown banners with emojis
  - [x] Global exception handling middleware
  - [x] Serilog with structured logging
  - [x] Real-time data tracking via RedisInsight
  - [x] Gateway integration completed
  - [x] Docker containerization
  - [x] Multi-stage Docker builds (.NET 9.0)
  - [x] **95 Unit Tests** (89 passed, 6 skipped) - xUnit, NSubstitute, FluentAssertions
    - Query handler tests (5 tests) - GetBasket with mapper mocking
    - Command handler tests (31 tests) - AddItem, RemoveItem, UpdateQuantity, ClearBasket
    - Validator tests (27 tests) - AddItem & UpdateQuantity validation rules
    - Domain entity tests (32 tests) - Basket & BasketItem business logic

- [x] **OrderService** - Event-Driven Architecture, Sipariş Yönetimi (Port: 5003)

  - [x] CQRS with MediatR pattern
  - [x] Clean Architecture (Domain, Application, Infrastructure, API)
  - [x] SQL Server database integration
  - [x] Order management endpoints:
    - CreateOrder - Sipariş oluşturma
    - GetOrder - Sipariş detayları
    - GetUserOrders - Kullanıcının tüm siparişleri
    - UpdateOrderStatus - Sipariş durumu güncelleme
    - CancelOrder - Sipariş iptali
  - [x] Domain-Driven Design:
    - Order aggregate root
    - OrderItem entity
    - Address & Money value objects
    - OrderStatus enum (7 durum)
  - [x] **Event-Driven Architecture:** 🆕
    - MassTransit 8.5.6 + RabbitMQ integration
    - **BasketCheckoutConsumer** (API Layer - Consumer = Controller pattern) 🆕
      - IBasketCheckoutEvent'i consume eder (basket-checkout-queue)
      - Event'i MediatR command'a dönüştürür
      - Retry policy: 3 deneme × 5 saniye
    - **Event Publishing:**
      - IOrderCreatedEvent - Sipariş oluşturulduğunda
      - IOrderStatusChangedEvent - Durum değiştiğinde
      - IOrderCompletedEvent - Sipariş tamamlandığında
      - IOrderCancelledEvent - Sipariş iptal edildiğinde
    - Anonymous type pattern ile event yayınlama
  - [x] **CreateOrderCommand Factory:** 🆕
    - FromBasketCheckoutEvent() static factory method
    - Event → Command dönüşümü
  - [x] FluentValidation:
    - Dynamic enum validation
    - Custom business rules
  - [x] AutoMapper 12.0.1 (version uyumluluğu)
  - [x] Advanced logging system:
    - LogHelper with emojis (🚀 ⚡ 💾 🐰)
    - LoggingBehavior & ValidationBehavior
    - Startup/Shutdown banners
    - Timer tracking
    - Consumer logging (basket checkout events)
  - [x] Global exception handling middleware
  - [x] Serilog structured logging
  - [x] Gateway integration
  - [x] Docker containerization
  - [x] Multi-stage Docker builds (.NET 9.0)
  - [x] AutoMapper 12.0.1 compatibility fix
  - [x] **84 Unit Tests** (100% pass) - xUnit, NSubstitute, FluentAssertions
    - Domain entity tests (16 tests) - Order, OrderItem, Address business logic
    - Command handler tests (20 tests) - CreateOrder (8), UpdateOrderStatus (7), CancelOrder (5)
    - Query handler tests (6 tests) - GetOrder, GetUserOrders with mapper mocking
    - Validator tests (42 tests) - CreateOrder & UpdateOrderStatus validation rules

- [x] **Shared Library** - Ortak Kütüphane ve Event Definitions 🆕

  - [x] **Event Interfaces:**
    - **Basket Events:** IBasketCheckoutEvent (sepet onaylama + DTO'lar)
    - **Order Events:** IOrderCreatedEvent, IOrderStatusChangedEvent, IOrderCompletedEvent, IOrderCancelledEvent
    - **Payment Events (Hazır):** IPaymentRequestEvent, IPaymentSuccessEvent, IPaymentFailedEvent
  - [x] **RabbitMQ Constants:** Queue names, connection settings, retry config (MaxRetryCount: 3)
  - [x] **Anonymous Type Pattern:** Interface-based contracts, concrete class'lara gerek yok
  - [x] **Servisler Arası İletişim:**
    - BasketService → OrderService (IBasketCheckoutEvent) ✅
    - OrderService → PaymentService (IOrderCreatedEvent - hazır)
    - PaymentService → OrderService (IPaymentSuccess/FailedEvent - hazır)
  - [x] .NET Standard 2.1 compatibility
  - [x] Kullanıldığı yerler: BasketService, OrderService, PaymentService (gelecek)

- [ ] **PaymentService** - Ödeme Entegrasyonu

## 🔄 Event-Driven Architecture Flow

### Sepet → Sipariş Akışı (Tamamlandı ✅)

1. **Kullanıcı sepeti onaylar** → BasketService CheckoutBasket endpoint
2. **BasketService** sepeti Redis'ten çeker, IBasketCheckoutEvent yayınlar (RabbitMQ'ya)
3. **RabbitMQ** event'i basket-checkout-queue'ya iletir
4. **OrderService** BasketCheckoutConsumer event'i consume eder
5. **OrderService** sipariş oluşturur (SQL Server), IOrderCreatedEvent yayınlar
6. **Sipariş başarıyla oluşturuldu**

### Gelecek Event Akışları

- **Sipariş → Ödeme:** OrderService IOrderCreatedEvent yayınlar → PaymentService consume eder
- **Ödeme → Sipariş:** PaymentService IPaymentSuccessEvent/FailedEvent yayınlar → OrderService status günceller

### 📋 Faz 5 - Destek Servisleri

- [ ] **NotificationService** - Event-Driven, Email/SMS

## Servis Port Yapısı

| Servis          | API Port | Database/Cache Port | UI Port | Durum |
| --------------- | -------- | ------------------- | ------- | ----- |
| ApiGateway      | 5050     | -                   | -       | ✅    |
| ProductService  | 5000     | 1450 (SQL Server)   | -       | ✅    |
| IdentityService | 5001     | 1450 (SQL Server)   | -       | ✅    |
| BasketService   | 5002     | 6379 (Redis)        | 5540    | ✅    |
| OrderService    | 5003     | 1450 (SQL Server)   | -       | ✅    |
| RabbitMQ        | 5672     | -                   | 15672   | ✅    |

## Swagger UI

- **API Gateway (Aggregated)**: http://localhost:5050/swagger/index.html ⭐ **Öneri: Buradan kullan!**
- **ProductService**: http://localhost:5000/swagger
- **IdentityService**: http://localhost:5001/swagger
- **BasketService**: http://localhost:5002/swagger
- **OrderService**: http://localhost:5003/swagger

## Yönetim Arayüzleri

### Redis Yönetimi

- **RedisInsight**: http://localhost:5540
  - Basket verilerini görsel olarak izleme
  - Key-value çiftlerini inceleme
  - Real-time data monitoring
  - Bağlantı ayarları: Host=`redis`, Port=`6379`

### RabbitMQ Yönetimi

- **RabbitMQ Management**: http://localhost:15672
  - Kullanıcı: `admin`
  - Şifre: `admin123`
  - Queue monitoring
  - Message tracking
  - Exchange & binding yönetimi

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

### 🐳 Docker ile Tüm Sistemi Başlatma (Önerilen)

Tüm mikroservisler Docker containerları içinde çalışacak şekilde yapılandırıldı. Multi-stage Docker builds kullanılarak optimize edilmiş image'lar oluşturuldu.

```bash
# Tüm servisleri ve altyapıyı tek komutla başlat
docker-compose up -d

# Çalışan servisleri kontrol et
docker ps

# Logları izle (tüm servisler)
docker-compose logs -f

# Belirli servislerin loglarını izle
docker-compose logs -f product-service basket-service order-service identity-service api-gateway

# Servisleri durdur
docker-compose down

# Servisleri durdur ve volume'ları sil
docker-compose down -v

# Tek bir servisi yeniden build et
docker-compose up -d --build product-service

# Sadece altyapı servislerini başlat
docker-compose up -d sqlserver redis rabbitmq redisinsight
```

**🎯 Dockerize Edilmiş Tüm Servisler:**

| Servis             | Port        | Container Name                    | Image Boyutu |
| ------------------ | ----------- | --------------------------------- | ------------ |
| **Mikroservisler** |
| ProductService     | 5000        | technology-store-product-service  | ~220MB       |
| IdentityService    | 5001        | technology-store-identity-service | ~220MB       |
| BasketService      | 5002        | technology-store-basket-service   | ~220MB       |
| OrderService       | 5003        | technology-store-order-service    | ~220MB       |
| ApiGateway         | 5050        | technology-store-api-gateway      | ~220MB       |
| **Altyapı**        |
| SQL Server 2022    | 1450        | technology-store-sqlserver        | -            |
| Redis Alpine       | 6379        | technology-store-redis            | -            |
| RabbitMQ           | 5672, 15672 | technology-store-rabbitmq         | -            |
| RedisInsight       | 5540        | technology-store-redisinsight     | -            |

**📦 Docker Build Stratejisi:**

Tüm mikroservisler için **multi-stage builds** kullanıldı:

1. **Build Stage:** `mcr.microsoft.com/dotnet/sdk:9.0` (veya 8.0) - Derleme için
2. **Runtime Stage:** `mcr.microsoft.com/dotnet/aspnet:9.0` (veya 8.0) - Çalıştırma için
3. **Sonuç:** ~1.2GB SDK yerine ~220MB runtime image

**🔧 Docker Context Yapılandırması:**

- **ProductService:** Context = `backend/src/Services` (Shared projesi yok)
- **IdentityService:** Context = `backend/src` (Shared projesi dahil)
- **BasketService:** Context = `backend/src` (Shared projesi dahil)
- **OrderService:** Context = `backend/src` (Shared projesi dahil)
- **ApiGateway:** Context = `backend/src/ApiGateway` (Standalone)

### 💻 Manuel Backend Çalıştırma (Development)

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

# OrderService (Port: 5003)
cd Services/OrderService/OrderService.API
dotnet run

# API Gateway (Port: 5050) - En son başlatın
cd ApiGateway
dotnet run
```

### 🗄️ Veritabanı Kontrolü

```powershell
# SQL Server'a bağlan
docker exec -it technology-store-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -C

# Veritabanlarını listele
SELECT name FROM sys.databases;
GO

# OrderService siparişlerini görüntüle
USE OrderServiceDb;
SELECT * FROM Orders ORDER BY CreatedDate DESC;
GO
```

### Frontend

```bash
cd frontend
npm install
npm run dev
```

## Teknolojiler

### Backend

- **.NET 8.0 & 9.0** (ProductService: 8.0, Others: 9.0)
- **Entity Framework Core** - ORM
- **MediatR** - CQRS pattern implementation
- **FluentValidation** - Request validation
- **AutoMapper** - Object-to-object mapping
- **Serilog** - Structured logging
- **xUnit, NSubstitute, FluentAssertions** - Unit testing
- **Ocelot** - API Gateway
- **Polly** - Resilience & Circuit Breaker
- **MassTransit 8.5.6** - Event-driven messaging abstraction
- **Swashbuckle (Swagger)** - API documentation

### Database & Cache

- **SQL Server 2022** - Relational database
- **Redis Alpine** - In-memory cache & data store
- **RedisInsight** - Redis GUI client

### Message Broker

- **RabbitMQ 3-management** - Message queue & event broker
- **MassTransit 8.5.6** - Messaging framework
  - Anonymous type event publishing
  - Consumer pattern (API layer entry point)
  - Retry policies & fault tolerance
  - Exchange & queue auto-configuration

### DevOps & Infrastructure

- **Docker** - Containerization
- **Docker Compose** - Multi-container orchestration
- **Multi-stage Docker builds** - Optimized image size
- **Docker networks** - Service communication
- **Health checks** - Container monitoring

### Frontend

- Next.js 14
- TypeScript
- React
