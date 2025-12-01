# Technology Store - Mikroservis Mimarisi

## Proje Yapısı

### Backend (Mikroservisler)

- **ApiGateway**: Tüm istekleri yönlendiren gateway ✅ **Tamamlandı**
- **IdentityService**: Kullanıcı kimlik doğrulama ve yetkilendirme ✅ **Tamamlandı**
- **ProductService**: Ürün ve kategori yönetimi ✅ **Tamamlandı**
- **BasketService**: Sepet yönetimi, Redis cache ve ödeme öncesi stok kontrolü ✅ **Tamamlandı**
- **OrderService**: Sipariş yönetimi ✅ **Tamamlandı**
- **PaymentService**: Ödeme işlemleri ✅ **Tamamlandı**
- **Shared**: Ortak kütüphaneler, event interface'leri ve RabbitMQ sabitleri ✅ **Tamamlandı**

## Geliştirme Sırası

### ✅ Faz 1 - Tamamlandı

- [x] **ProductService** - CQRS, Clean Architecture, Stok Yönetimi (Port: 5000)
  - [x] CQRS with MediatR pattern
  - [x] Clean Architecture (Domain, Application, Infrastructure, API)
  - [x] Product & Category CRUD operations
  - [x] 47 Unit Tests (100% pass)
  - [x] **Stok Yönetimi:** 🆕
    - Product.DecreaseStock() / IncreaseStock() - Domain layer business logic
    - DecreaseProductStockCommand/Handler - CQRS command pattern
    - CheckStockQuery/Handler - Toplu stok doğrulama
    - StockController - REST API endpoint (POST /api/stock/check)
  - [x] **Event-Driven Stok Güncelleme:** 🆕
    - OrderCreatedConsumer - Sipariş oluşturulduğunda stok düşürme
    - IOrderCreatedEvent consume eder (order-created-queue)
    - Her sipariş kalemi için otomatik stok azaltma
    - Başarısız stok güncellemeleri loglama
  - [x] Docker containerization with multi-stage builds

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

- [x] **BasketService** - Redis Cache, Sepet Yönetimi ve Stok Kontrolü (Port: 5002)

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
  - [x] **Ödeme Öncesi Stok Kontrolü:** 🆕
    - ProductServiceClient - HTTP client ile ProductService entegrasyonu
    - CheckoutBasket sırasında gerçek zamanlı stok doğrulama
    - Yetersiz stok durumunda sipariş oluşturulmadan hata döner (400 BadRequest)
    - Detaylı stok hata mesajları: "iPhone (need 5, have 2)"
    - Mikroservisler arası senkron HTTP iletişimi
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
    - **Payment Events:** IPaymentSuccessEvent, IPaymentFailedEvent
  - [x] **RabbitMQ Constants:** Queue names, connection settings, retry config (MaxRetryCount: 3)
  - [x] **Anonymous Type Pattern:** Interface-based contracts, concrete class'lara gerek yok
  - [x] **Servisler Arası İletişim:**
    - BasketService → OrderService (IBasketCheckoutEvent) ✅
    - OrderService → PaymentService (IOrderCreatedEvent) ✅
    - PaymentService → OrderService (IPaymentSuccess/FailedEvent) ✅
  - [x] .NET Standard 2.1 compatibility
  - [x] Kullanıldığı yerler: BasketService, OrderService, PaymentService

- [x] **PaymentService** - Ödeme Yönetimi ve Sahte Ödeme Gateway'i (Port: 5004)

  - [x] CQRS with MediatR pattern
  - [x] Clean Architecture (Domain, Application, Infrastructure, API)
  - [x] SQL Server database integration
  - [x] Payment management endpoints:
    - GetPayment - Ödeme detayları
    - GetPaymentsByUserId - Kullanıcının ödemeleri
  - [x] Domain-Driven Design:
    - Payment aggregate root (OrderId, UserId, Amount, Status, TransactionId)
    - Money value object (Amount + Currency)
    - PaymentStatus enum (Pending, Processing, Success, Failed, Refunded)
    - **Idempotency Check** - Aynı sipariş için tekrar ödeme alınmasını önler
  - [x] **FakePaymentGateway** - Mock banka entegrasyonu 🆕
    - %90 başarı oranı (gerçekçi senaryo)
    - 1 saniye ağ gecikmesi simülasyonu
    - 5 farklı hata senaryosu: "Yetersiz bakiye", "Kart reddedildi", "Geçersiz kart", "Banka zaman aşımı", "Günlük limit aşımı"
    - Gerçek banka API'si olmadan test yapılabilir
  - [x] **Event-Driven Architecture:** 🆕
    - MassTransit 8.5.6 + RabbitMQ integration
    - **OrderCreatedConsumer** (API Layer - Consumer = Controller pattern) 🆕
      - IOrderCreatedEvent'i consume eder (order-created-queue)
      - **Idempotency kontrolü** - Sipariş daha önce işlendiyse atla
      - FakePaymentGateway ile ödeme işler
      - Event'i MediatR command'a dönüştürür
      - Retry policy: 3 deneme × 5 saniye
    - **Event Publishing:**
      - IPaymentSuccessEvent - Ödeme başarılı (PaymentIntentId, PaymentMethod, CompletedDate)
      - IPaymentFailedEvent - Ödeme başarısız (Reason, FailedDate)
    - Anonymous type pattern ile event yayınlama
  - [x] **OrderService Payment Feedback Loop:** 🆕
    - **PaymentSuccessConsumer** - Ödeme başarılı → Sipariş durumu "PaymentReceived" olur
    - **PaymentFailedConsumer** - Ödeme başarısız → Sipariş durumu "Failed" olur
    - payment-success-queue ve payment-failed-queue kuyrukları
  - [x] FluentValidation with custom validators
  - [x] AutoMapper 12.0.1 entity-DTO mapping
  - [x] Advanced logging system:
    - LogHelper with emojis (💰 💳 ⚡)
    - LoggingBehavior & ValidationBehavior
    - Startup/Shutdown banners
    - Consumer logging (order created & payment result events)
  - [x] Global exception handling middleware
  - [x] Serilog structured logging
  - [x] Gateway integration (hazır)
  - [x] Docker containerization
  - [x] Multi-stage Docker builds (.NET 9.0)
  - [x] **73 Unit Tests** (100% pass) - xUnit, NSubstitute, FluentAssertions
    - Domain entity tests (18 tests) - Payment state machine, idempotency checks
    - Domain value object tests (25 tests) - Money operators, validation, formatting
    - Command handler tests (10 tests) - ProcessPayment with gateway integration, event publishing
    - Query handler tests (20 tests) - GetPaymentById, GetPaymentsByUserId with mapper mocking

## 🔄 Event-Driven Architecture Flow (Tam Akış)

### 1️⃣ Sepet → Stok Kontrolü → Sipariş → Ödeme → Stok Güncelleme (Tamamlandı ✅)

**Başarılı Akış:**

1. **Kullanıcı sepeti onaylar** → BasketService `POST /api/baskets/{id}/checkout`
2. **BasketService** stok kontrolü yapar → ProductService `POST /api/stock/check` (HTTP)
3. **ProductService** tüm ürünlerin stok durumunu kontrol eder
4. **Stok Yetersiz İse:** ❌ 400 BadRequest döner, sipariş oluşturulmaz
5. **Stok Yeterli İse:** ✅ BasketService sepeti Redis'ten çeker, `IBasketCheckoutEvent` yayınlar → `basket-checkout-queue`
6. **OrderService.BasketCheckoutConsumer** event'i consume eder
7. **OrderService** sipariş oluşturur (Status: **Pending**), `IOrderCreatedEvent` yayınlar → `order-created-queue`
8. **PaymentService.OrderCreatedConsumer** event'i consume eder
9. **PaymentService** idempotency kontrolü yapar (aynı sipariş daha önce işlendiyse atlar)
10. **FakePaymentGateway** ödemeyi işler (%90 başarı, 1 saniye gecikme)
11. **Ödeme Başarılı:** `IPaymentSuccessEvent` yayınlar → `payment-success-queue`
12. **OrderService.PaymentSuccessConsumer** event'i consume eder
13. **OrderService** sipariş durumunu **PaymentReceived** olarak günceller
14. **ProductService.OrderCreatedConsumer** event'i consume eder (paralel)
15. **ProductService** her ürün için stok düşürür (DecreaseProductStockCommand)
16. ✅ **Sipariş tamamlandı - Ödeme alındı - Stok güncellendi!**

**Başarısız Akış (Yetersiz Stok):**

1. Kullanıcı sepeti onaylar
2. BasketService stok kontrolü yapar
3. **ProductService:** ❌ Stok yetersiz (örn: "iPhone need 5, have 2")
4. **BasketService:** 400 BadRequest döner, detaylı hata mesajı
5. Sipariş oluşturulmaz, ödeme alınmaz
6. ❌ **Checkout iptal - Kullanıcı bilgilendirildi**

**Başarısız Akış (Ödeme Hatası):**

1-10. Yukarıdaki adımlar aynı 11. **Ödeme Başarısız:** `IPaymentFailedEvent` yayınlar → `payment-failed-queue` (Reason: "Yetersiz bakiye" vb.) 12. **OrderService.PaymentFailedConsumer** event'i consume eder 13. **OrderService** sipariş durumunu **Failed** olarak günceller 14. **ProductService** stok düşürme yapmaz (OrderCreatedEvent dinlemedi çünkü ödeme başarısız) 15. ❌ **Sipariş başarısız - Ödeme alınamadı**

### 2️⃣ Sipariş Durum Döngüsü

```
Pending (Ödeme bekleniyor)
   ↓ (Ödeme başarılı)
PaymentReceived (Ödeme alındı)
   ↓ (Depo hazırlık)
Processing (Sipariş hazırlanıyor)
   ↓
Shipped (Kargoya verildi)
   ↓
Delivered (Teslim edildi)

   ↓ (Ödeme başarısız)
Failed (Sipariş başarısız)
   ↓ (İptal)
Cancelled (İptal edildi)
```

### 3️⃣ Event Kuyrukları

- **basket-checkout-queue** → BasketService → OrderService
- **order-created-queue** → OrderService → PaymentService + ProductService (paralel)
- **payment-success-queue** → PaymentService → OrderService
- **payment-failed-queue** → PaymentService → OrderService

### 4️⃣ Mikroservis İletişim Stratejileri

**Senkron HTTP İletişimi (Request-Response):**

- BasketService → ProductService (Stok kontrolü)
- Kullanım: Gerçek zamanlı doğrulama, hızlı feedback gerekli durumlar
- Avantaj: Anlık sonuç, basit hata yönetimi
- Dezavantaj: Servisler arası coupling, latency

**Asenkron Event-Driven İletişim (Publish-Subscribe):**

- OrderService → ProductService (Stok güncelleme)
- Kullanım: Fire-and-forget, eventual consistency kabul edilebilir durumlar
- Avantaj: Loose coupling, scalability, resilience
- Dezavantaj: Eventual consistency, retry mekanizması gerekli

### 5️⃣ Retry Politikası

Tüm consumerlar 3 deneme × 5 saniye retry policy ile korunur.

## Servis Port Yapısı

| Servis          | API Port | Database/Cache Port | UI Port | Durum |
| --------------- | -------- | ------------------- | ------- | ----- |
| ApiGateway      | 5050     | -                   | -       | ✅    |
| ProductService  | 5000     | 1450 (SQL Server)   | -       | ✅    |
| IdentityService | 5001     | 1450 (SQL Server)   | -       | ✅    |
| BasketService   | 5002     | 6379 (Redis)        | 5540    | ✅    |
| OrderService    | 5003     | 1450 (SQL Server)   | -       | ✅    |
| PaymentService  | 5004     | 1450 (SQL Server)   | -       | ✅    |
| RabbitMQ        | 5672     | -                   | 15672   | ✅    |

## Swagger UI

- **API Gateway (Aggregated)**: http://localhost:5050/swagger/index.html ⭐ **Öneri: Buradan kullan!**
- **ProductService**: http://localhost:5000/swagger
- **IdentityService**: http://localhost:5001/swagger
- **BasketService**: http://localhost:5002/swagger
- **OrderService**: http://localhost:5003/swagger
- **PaymentService**: http://localhost:5004/swagger

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
| PaymentService     | 5004        | technology-store-payment-service  | ~220MB       |
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
- **PaymentService:** Context = `backend/src` (Shared projesi dahil)
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

# PaymentService (Port: 5004)
cd Services/PaymentService/PaymentService.API
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

# PaymentService ödemelerini görüntüle
USE PaymentServiceDb;
SELECT * FROM Payments ORDER BY CreatedDate DESC;
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
