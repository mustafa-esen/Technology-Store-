# TechnologyStore.Shared - Event-Driven Architecture Documentation

## 📋 Genel Bakış

Bu proje, Technology Store mikroservis mimarisindeki tüm servislerin kullandığı ortak event'leri, DTO'ları ve sabitleri içerir. **MassTransit best practice** olarak tüm event'ler **interface** olarak tanımlanmıştır.

## 🎯 Event-Driven Architecture Akış Şeması

```
┌─────────────────┐
│  BasketService  │
│  (Sepet)        │
└────────┬────────┘
         │ Publish: IBasketCheckoutEvent
         ↓
┌─────────────────┐
│  OrderService   │
│  (Sipariş)      │◄────────┐
└────────┬────────┘         │
         │ Publish:         │
         │ • IOrderCreatedEvent
         │ • IOrderStatusChangedEvent
         │ • IOrderCompletedEvent
         │ • IOrderCancelledEvent
         │                  │
         ↓                  │
    ┌────────┴────────┐    │
    │                 │    │
    ↓                 ↓    │
┌──────────┐    ┌──────────────┐
│ Payment  │    │ Notification │
│ Service  │    │   Service    │
└────┬─────┘    └──────────────┘
     │ Publish:
     │ • IPaymentSuccessEvent
     │ • IPaymentFailedEvent
     └──────────────┘
```

## 📁 Proje Yapısı

```
TechnologyStore.Shared/
├── Constants/
│   ├── RabbitMqConstants.cs      # Queue, Exchange, Routing Key sabitleri
│   ├── ApiRoutes.cs               # API endpoint sabitleri
│   └── Messages.cs                # Hata ve başarı mesajları
├── Events/
│   ├── Baskets/
│   │   └── BasketEvents.cs       # IBasketCheckoutEvent
│   ├── Orders/
│   │   ├── OrderCreatedEvent.cs  # IOrderCreatedEvent
│   │   ├── OrderStatusChangedEvent.cs
│   │   ├── OrderCompletedEvent.cs
│   │   └── OrderCancelledEvent.cs
│   ├── Payments/
│   │   └── PaymentEvents.cs      # IPaymentSuccessEvent, IPaymentFailedEvent
│   └── Identity/
│       └── IdentityEvents.cs     # IUserRegisteredEvent, IUserLoggedInEvent
```

## 🔄 Event Akışları

### 1️⃣ Basket (Sepet) Checkout Akışı

**Event:** `IBasketCheckoutEvent`

- **Publisher:** BasketService
- **Consumer:** OrderService
- **Amaç:** Kullanıcı "Satın Al" dediğinde sepet verilerini OrderService'e taşımak

```csharp
public interface IBasketCheckoutEvent
{
    string UserId { get; set; }
    string UserName { get; set; }
    decimal TotalPrice { get; set; }
    BasketCheckoutAddressDto ShippingAddress { get; set; }
    List<BasketItemDto> Items { get; set; }
    DateTime CheckedOutDate { get; set; }
}
```

### 2️⃣ Order (Sipariş) Oluşturma Akışı

**Event:** `IOrderCreatedEvent`

- **Publisher:** OrderService
- **Consumers:** PaymentService, NotificationService, ProductService (Stok)
- **Amaç:** Ödeme ve stok işlemlerini başlatmak

```csharp
public interface IOrderCreatedEvent
{
    Guid OrderId { get; set; }
    string UserId { get; set; }
    decimal TotalAmount { get; set; }
    List<OrderItemDto> Items { get; set; }
    ShippingAddressDto ShippingAddress { get; set; }
    DateTime CreatedDate { get; set; }
}
```

### 3️⃣ Order (Sipariş) Durum Değişikliği

**Event:** `IOrderStatusChangedEvent`

- **Publisher:** OrderService
- **Consumer:** NotificationService
- **Amaç:** Kullanıcıyı sipariş durumu hakkında bilgilendirmek

```csharp
public interface IOrderStatusChangedEvent
{
    Guid OrderId { get; set; }
    string UserId { get; set; }
    OrderStatus OldStatus { get; set; }
    OrderStatus NewStatus { get; set; }
    DateTime ChangedDate { get; set; }
}
```

### 4️⃣ Order (Sipariş) Tamamlanma

**Event:** `IOrderCompletedEvent`

- **Publisher:** OrderService
- **Consumer:** NotificationService
- **Amaç:** Kullanıcıya "Siparişiniz tamamlandı" bildirimi göndermek

### 5️⃣ Order (Sipariş) İptali

**Event:** `IOrderCancelledEvent`

- **Publisher:** OrderService
- **Consumers:** NotificationService, ProductService (Stok iadesi)
- **Amaç:** İptal süreçlerini yönetmek ve stoğu geri iade etmek

```csharp
public interface IOrderCancelledEvent
{
    Guid OrderId { get; set; }
    string UserId { get; set; }
    string Reason { get; set; }
    DateTime CancelledDate { get; set; }
}
```

### 6️⃣ Payment (Ödeme) Başarılı

**Event:** `IPaymentSuccessEvent`

- **Publisher:** PaymentService
- **Consumer:** OrderService
- **Amaç:** Sipariş durumunu "Paid" olarak güncellemek

```csharp
public interface IPaymentSuccessEvent
{
    Guid OrderId { get; set; }
    string UserId { get; set; }
    decimal Amount { get; set; }
    string PaymentIntentId { get; set; }
    string PaymentMethod { get; set; }
    DateTime CompletedDate { get; set; }
}
```

### 7️⃣ Payment (Ödeme) Başarısız

**Event:** `IPaymentFailedEvent`

- **Publisher:** PaymentService
- **Consumer:** OrderService
- **Amaç:** Sipariş durumunu "PaymentFailed" yapmak ve siparişi iptal etmek

```csharp
public interface IPaymentFailedEvent
{
    Guid OrderId { get; set; }
    string UserId { get; set; }
    decimal Amount { get; set; }
    string Reason { get; set; }
    DateTime FailedDate { get; set; }
}
```

### 8️⃣ User (Kullanıcı) Kaydı

**Event:** `IUserRegisteredEvent`

- **Publisher:** IdentityService
- **Consumer:** NotificationService
- **Amaç:** "Hoşgeldiniz" maili göndermek

```csharp
public interface IUserRegisteredEvent
{
    string UserId { get; set; }
    string Email { get; set; }
    string FullName { get; set; }
    DateTime RegisteredDate { get; set; }
}
```

## 🛠️ RabbitMQ Constants Kullanımı

### Queue İsimleri

```csharp
using TechnologyStore.Shared.Constants;

// MassTransit konfigürasyonunda kullanım
cfg.ReceiveEndpoint(RabbitMqConstants.OrderCreatedQueue, e =>
{
    e.ConfigureConsumer<OrderCreatedConsumer>(context);
});
```

### Örnek Queue İsimleri

| Constant                        | Queue Name                       |
| ------------------------------- | -------------------------------- |
| `BasketCheckoutQueue`           | basket-checkout-queue            |
| `OrderCreatedQueue`             | order-created-queue              |
| `PaymentSuccessQueue`           | payment-success-queue            |
| `PaymentFailedQueue`            | payment-failed-queue             |
| `NotificationOrderCreatedQueue` | notification-order-created-queue |
| `StockOrderCreatedQueue`        | stock-order-created-queue        |
| `UserRegisteredQueue`           | user-registered-queue            |

## 📝 API Routes Kullanımı

```csharp
using TechnologyStore.Shared.Constants;

// Controller'da kullanım
[HttpGet(ApiRoutes.Orders.GetById)]
public async Task<IActionResult> GetOrder(Guid orderId)
{
    // ...
}

// Gateway Ocelot configuration'da kullanım
"UpstreamPathTemplate": ApiRoutes.Orders.GetById,
"DownstreamPathTemplate": ApiRoutes.Orders.GetById,
```

## 💬 Mesaj Sabitleri Kullanımı

```csharp
using TechnologyStore.Shared.Constants;

// Hata mesajı döndürme
if (order == null)
{
    return NotFound(ErrorMessages.Order.NotFound);
}

// Başarı mesajı döndürme
return Ok(new
{
    Message = SuccessMessages.Order.Created,
    Data = orderDto
});
```

## 🔄 Tam Senaryo: Sepetten Siparişe

```
1. Kullanıcı "Satın Al" → BasketService.Checkout()
   ↓
2. BasketService → Publish(IBasketCheckoutEvent)
   ↓
3. OrderService → Consume(IBasketCheckoutEvent)
   → Siparişi oluştur (Status: Pending)
   → Publish(IOrderCreatedEvent)
   ↓
4. PaymentService → Consume(IOrderCreatedEvent)
   → Ödemeyi işle
   ├─ Başarılı → Publish(IPaymentSuccessEvent)
   └─ Başarısız → Publish(IPaymentFailedEvent)
   ↓
5. OrderService → Consume(IPaymentSuccessEvent/IPaymentFailedEvent)
   ├─ Success → Status = "Completed" → Publish(IOrderCompletedEvent)
   └─ Failed → Status = "Cancelled" → Publish(IOrderCancelledEvent)
   ↓
6. NotificationService → Consume(tüm eventleri dinler)
   → Kullanıcıya e-posta/SMS gönder
```

## ✅ Best Practices

### 1. Interface Kullanımı

✅ **Doğru:**

```csharp
public interface IOrderCreatedEvent { }
public class OrderCreatedEvent : IOrderCreatedEvent { }
```

❌ **Yanlış:**

```csharp
public class OrderCreatedEvent { } // Interface yok!
```

### 2. Publisher/Consumer Dokümantasyonu

Her interface üzerinde XML comment ile belirtilmelidir:

```csharp
/// <summary>
/// Sipariş oluşturulduğunda yayınlanan event
/// Publisher: OrderService
/// Consumers: PaymentService, NotificationService
/// </summary>
public interface IOrderCreatedEvent { }
```

### 3. DTO Kullanımı

Event'lerde doğrudan entity yerine DTO kullanın:

```csharp
// ✅ DTO kullanımı
public List<OrderItemDto> Items { get; set; }

// ❌ Entity kullanımı
public List<OrderItem> Items { get; set; }
```

### 4. Constant Kullanımı

Hard-coded string yerine constant kullanın:

```csharp
// ✅ Constant kullanımı
cfg.ReceiveEndpoint(RabbitMqConstants.OrderCreatedQueue, ...);

// ❌ Hard-coded string
cfg.ReceiveEndpoint("order-created-queue", ...);
```

## 📦 NuGet Dependencies

```xml
<PackageReference Include="MassTransit" Version="8.x.x" />
<PackageReference Include="MassTransit.RabbitMQ" Version="8.x.x" />
```

## 🎯 Gelecek Geliştirmeler

- [ ] Stock/Inventory events (IStockReservedEvent, IStockReleasedEvent)
- [ ] Shipment events (IOrderShippedEvent, IOrderDeliveredEvent)
- [ ] Analytics events (IProductViewedEvent, ISearchPerformedEvent)
- [ ] Review events (IReviewCreatedEvent, IReviewApprovedEvent)

---

**Not:** Bu dokümantasyon, Technology Store projesindeki event-driven architecture'ın temelini oluşturur. Yeni servisler eklenirken bu yapıya uygun event'ler eklenmelidir.
