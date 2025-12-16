# API Gateway - Technology Store

API Gateway, tüm mikroservislere tek bir giriş noktası sağlar. Ocelot ile route yönetimi, Polly ile dayanıklılık (resilience) ve SwaggerForOcelot ile aggregate API dokümantasyonu sunar.

## Özellikler

### ✅ Routing & Load Balancing

- **Ocelot 24.0.1**: API Gateway core routing engine
- **Routes**:
  - Product Service (5000): 5 route
  - Identity Service (5001): 3 route
- **Base URL**: http://localhost:5050

### ✅ Resilience Patterns (Polly)

- **Circuit Breaker**: 3 başarısız istek sonrası 10 saniye devre dışı
- **Timeout**: Her istek için 5 saniye timeout
- **Retry**: Otomatik yeniden deneme

### ✅ Aggregated Swagger Documentation

- **MMLib.SwaggerForOcelot 9.0.0**: Tüm mikroservislerin Swagger dokümantasyonunu tek bir UI'da toplar
- **URL**: http://localhost:5050/swagger
- **Transformer**: Servis portlarını (5000, 5001) Gateway portuna (5050) otomatik dönüştürür

### ✅ Logging

- **Serilog**: Yapılandırılmış loglama
- **Console Sink**: Konsola renkli log çıktısı
- **File Sink**: Günlük rotating log dosyaları (Logs/gateway-.log)

## Yapılandırma Dosyaları

### ocelot.json

Gateway route tanımlamaları ve QoS konfigürasyonu:

```json
{
  "Routes": [
    // Product Service Routes
    { "UpstreamPathTemplate": "/api/products", ... },
    { "UpstreamPathTemplate": "/api/products/{id}", ... },
    { "UpstreamPathTemplate": "/api/products/category/{categoryId}", ... },
    { "UpstreamPathTemplate": "/api/categories", ... },
    { "UpstreamPathTemplate": "/api/categories/{id}", ... },

    // Identity Service Routes
    { "UpstreamPathTemplate": "/api/auth/register", ... },
    { "UpstreamPathTemplate": "/api/auth/login", ... },
    { "UpstreamPathTemplate": "/api/auth/refresh-token", ... }
  ],
  "QoSOptions": {
    "ExceptionsAllowedBeforeBreaking": 3,
    "DurationOfBreak": 10000,
    "TimeoutValue": 5000
  },
  "GlobalConfiguration": {
    "BaseUrl": "http://localhost:5050"
  }
}
```

### ocelot.SwaggerEndPoints.json

Swagger endpoint tanımlamaları:

```json
[
  {
    "Key": "Products",
    "Config": [
      {
        "Name": "Product Service API",
        "Version": "v1",
        "Url": "http://localhost:5000/swagger/v1/swagger.json"
      }
    ]
  },
  {
    "Key": "Identity",
    "Config": [
      {
        "Name": "Identity Service API",
        "Version": "v1",
        "Url": "http://localhost:5001/swagger/v1/swagger.json"
      }
    ]
  }
]
```

## Çalıştırma

### 1. Docker Container'ları Başlat

```bash
docker-compose up -d
```

### 2. Mikroservisleri Başlat

```bash
# Terminal 1 - Product Service
cd backend/src/Services/ProductService/ProductService.API
dotnet run

# Terminal 2 - Identity Service
cd backend/src/Services/IdentityService/IdentityService.API
dotnet run

# Terminal 3 - API Gateway
cd backend/src/ApiGateway/ApiGateway
dotnet run
```

### 3. Test Et

- **Swagger UI**: http://localhost:5050/swagger
- **API Gateway**: http://localhost:5050
- **Test dosyası**: `ApiGateway.http` (VS Code REST Client)

## Route Örnekleri

### Product Service Routes

```http
GET http://localhost:5050/api/products
GET http://localhost:5050/api/products/1
GET http://localhost:5050/api/products/category/1
GET http://localhost:5050/api/categories
GET http://localhost:5050/api/categories/1
```

### Identity Service Routes

```http
POST http://localhost:5050/api/auth/register
POST http://localhost:5050/api/auth/login
POST http://localhost:5050/api/auth/refresh-token
```

## Polly QoS Davranışı

### Circuit Breaker

1. **Normal**: Tüm istekler downstream servise gönderilir
2. **Open** (Devre Açık): 3 başarısız istek sonrası devre açılır, 10 saniye servis çağrılmaz
3. **Half-Open**: 10 saniye sonra test isteği gönderilir
4. **Closed** (Devre Kapalı): Test başarılıysa normal moda döner

### Timeout

Her istek için maksimum 5 saniye beklenir. Bu süre aşılırsa `TimeoutException` fırlatılır.

## Gelecek Geliştirmeler

- [ ] **JWT Authentication**: Gateway seviyesinde JWT token validation
- [ ] **Rate Limiting**: İstek sınırlama (DDoS koruması)
- [ ] **Health Checks**: Mikroservislerin health check endpoint'lerini izleme
- [ ] **Response Caching**: Sık kullanılan endpoint'ler için cache
- [ ] **Request Aggregation**: Birden fazla servisi tek istekte birleştirme
- [ ] **API Versioning**: Route seviyesinde versiyon yönetimi

## Teknolojiler

- **ASP.NET Core 9.0**
- **Ocelot 24.0.1**: API Gateway routing
- **Ocelot.Provider.Polly 24.0.0**: Resilience patterns
- **MMLib.SwaggerForOcelot 9.0.0**: Swagger aggregation
- **Serilog 10.0.0**: Structured logging

## Port Yapısı

| Servis          | Port |
| --------------- | ---- |
| API Gateway     | 5050 |
| ProductService  | 5000 |
| IdentityService | 5001 |

---

**Not**: API Gateway'i çalıştırmadan önce downstream servislerin (ProductService, IdentityService) çalışır durumda olması gerekir. Aksi takdirde Polly Circuit Breaker devreye girer ve servis çağrıları engellenebilir.
