namespace OrderService.Domain.Enums;

public enum OrderStatus
{
    Pending = 0,           // Ödeme bekleniyor
    PaymentReceived = 1,   // Ödeme alındı
    Processing = 2,        // Sipariş hazırlanıyor
    Shipped = 3,           // Kargoya verildi
    Delivered = 4,         // Teslim edildi
    Cancelled = 5,         // İptal edildi
    Failed = 6             // Başarısız
}
