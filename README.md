# Technology Store - Mikroservis Mimarisi

## Proje Yapısı

### Backend (Mikroservisler)

- **ApiGateway**: Tüm istekleri yönlendiren gateway
- **IdentityService**: Kullanıcı kimlik doğrulama ve yetkilendirme
- **ProductService**: Ürün yönetimi
- **OrderService**: Sipariş yönetimi
- **PaymentService**: Ödeme işlemleri
- **NotificationService**: E-posta ve SMS bildirimleri
- **Shared**: Ortak kütüphaneler ve modeller

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
- Kubernetes
- RabbitMQ
- Redis
- Entity Framework Core
