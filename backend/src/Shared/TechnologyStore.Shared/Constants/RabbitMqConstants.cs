namespace TechnologyStore.Shared.Constants;

/// RabbitMQ kuyruk isimleri ve exchange'leri için sabitler
/// Bu sabitler tüm mikroservislerde tutarlı kuyruk isimlendirmesi sağlar
public static class RabbitMqConstants
{
    #region Queue Names

    // Basket Related Queues
    public const string BasketCheckoutQueue = "basket-checkout-queue";

    // Order Related Queues
    public const string OrderCreatedQueue = "order-created-queue";
    public const string OrderStatusChangedQueue = "order-status-changed-queue";
    public const string OrderCompletedQueue = "order-completed-queue";
    public const string OrderCancelledQueue = "order-cancelled-queue";

    // Payment Related Queues
    public const string PaymentRequestedQueue = "payment-requested-queue";
    public const string PaymentSuccessQueue = "payment-success-queue";
    public const string PaymentFailedQueue = "payment-failed-queue";

    // Identity Related Queues
    public const string UserRegisteredQueue = "user-registered-queue";
    public const string UserLoggedInQueue = "user-logged-in-queue";

    // Notification Service Queues (Multiple consumers için ayrı kuyruklar)
    public const string NotificationOrderCreatedQueue = "notification-order-created-queue";
    public const string NotificationOrderStatusChangedQueue = "notification-order-status-changed-queue";
    public const string NotificationOrderCancelledQueue = "notification-order-cancelled-queue";
    public const string NotificationUserRegisteredQueue = "notification-user-registered-queue";

    // Stock Management Queues (ProductService için)
    public const string StockOrderCreatedQueue = "stock-order-created-queue";
    public const string StockOrderCancelledQueue = "stock-order-cancelled-queue";

    #endregion

    #region Exchange Names

    // Exchange'ler event bazlı iletişim için
    public const string BasketExchange = "basket-exchange";
    public const string OrderExchange = "order-exchange";
    public const string PaymentExchange = "payment-exchange";
    public const string IdentityExchange = "identity-exchange";
    public const string NotificationExchange = "notification-exchange";
    public const string StockExchange = "stock-exchange";

    #endregion

    #region Routing Keys

    // Routing keys for more granular message routing
    public const string BasketCheckoutRoutingKey = "basket.checkout";

    public const string OrderCreatedRoutingKey = "order.created";
    public const string OrderStatusChangedRoutingKey = "order.status.changed";
    public const string OrderCompletedRoutingKey = "order.completed";
    public const string OrderCancelledRoutingKey = "order.cancelled";

    public const string PaymentRequestedRoutingKey = "payment.requested";
    public const string PaymentSuccessRoutingKey = "payment.success";
    public const string PaymentFailedRoutingKey = "payment.failed";

    public const string UserRegisteredRoutingKey = "user.registered";
    public const string UserLoggedInRoutingKey = "user.loggedin";

    #endregion

    #region Connection Settings

    // Connection configuration keys
    public const string DefaultVirtualHost = "/";
    public const int DefaultPort = 5672;
    public const int ManagementPort = 15672;

    // Retry settings
    public const int MaxRetryCount = 3;
    public const int RetryDelaySeconds = 5;

    #endregion
}
