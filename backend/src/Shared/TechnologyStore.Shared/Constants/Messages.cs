namespace TechnologyStore.Shared.Constants;

/// Hata mesajları ve durum kodları için sabitler
/// Tüm mikroservisler arasında tutarlı hata yönetimi sağlar
public static class ErrorMessages
{
    #region Generic Errors
    public const string InternalServerError = "An unexpected error occurred. Please try again later.";
    public const string ValidationError = "Validation failed. Please check your input.";
    public const string NotFound = "The requested resource was not found.";
    public const string Unauthorized = "You are not authorized to perform this action.";
    public const string Forbidden = "Access to this resource is forbidden.";
    public const string BadRequest = "The request was invalid or cannot be served.";
    #endregion

    #region Product Service Errors
    public static class Product
    {
        public const string NotFound = "Product not found.";
        public const string AlreadyExists = "Product with this name already exists.";
        public const string OutOfStock = "Product is out of stock.";
        public const string InsufficientStock = "Insufficient stock available.";
        public const string InvalidPrice = "Product price must be greater than zero.";
    }

    public static class Category
    {
        public const string NotFound = "Category not found.";
        public const string AlreadyExists = "Category with this name already exists.";
        public const string HasProducts = "Cannot delete category with existing products.";
        public const string InvalidName = "Category name is required.";
    }
    #endregion

    #region Identity Service Errors
    public static class Identity
    {
        public const string InvalidCredentials = "Invalid email or password.";
        public const string UserNotFound = "User not found.";
        public const string EmailAlreadyExists = "Email address is already registered.";
        public const string WeakPassword = "Password must be at least 6 characters long.";
        public const string InvalidToken = "Invalid or expired token.";
        public const string TokenExpired = "Token has expired. Please login again.";
        public const string PasswordMismatch = "Current password is incorrect.";
    }
    #endregion

    #region Basket Service Errors
    public static class Basket
    {
        public const string NotFound = "Basket not found.";
        public const string Empty = "Basket is empty.";
        public const string ItemNotFound = "Item not found in basket.";
        public const string InvalidQuantity = "Quantity must be greater than zero.";
        public const string ProductNotAvailable = "Product is no longer available.";
    }
    #endregion

    #region Order Service Errors
    public static class Order
    {
        public const string NotFound = "Order not found.";
        public const string EmptyOrder = "Cannot create order with no items.";
        public const string InvalidStatus = "Invalid order status.";
        public const string CannotCancel = "Order cannot be cancelled in its current state.";
        public const string AlreadyCancelled = "Order is already cancelled.";
        public const string AlreadyCompleted = "Order is already completed.";
        public const string InvalidAddress = "Shipping address is required.";
    }
    #endregion

    #region Payment Service Errors
    public static class Payment
    {
        public const string Failed = "Payment processing failed. Please try again.";
        public const string Declined = "Payment was declined by your bank.";
        public const string InvalidAmount = "Payment amount must be greater than zero.";
        public const string OrderNotFound = "Associated order not found.";
        public const string AlreadyPaid = "Order is already paid.";
        public const string RefundFailed = "Refund processing failed.";
        public const string InvalidPaymentMethod = "Invalid payment method.";
    }
    #endregion
}

/// Başarı mesajları için sabitler
public static class SuccessMessages
{
    #region Generic Success
    public const string OperationSuccessful = "Operation completed successfully.";
    public const string Created = "Resource created successfully.";
    public const string Updated = "Resource updated successfully.";
    public const string Deleted = "Resource deleted successfully.";
    #endregion

    #region Product Service Success
    public static class Product
    {
        public const string Created = "Product created successfully.";
        public const string Updated = "Product updated successfully.";
        public const string Deleted = "Product deleted successfully.";
    }

    public static class Category
    {
        public const string Created = "Category created successfully.";
        public const string Updated = "Category updated successfully.";
        public const string Deleted = "Category deleted successfully.";
    }
    #endregion

    #region Identity Service Success
    public static class Identity
    {
        public const string RegistrationSuccess = "Registration completed successfully.";
        public const string LoginSuccess = "Login successful.";
        public const string LogoutSuccess = "Logout successful.";
        public const string TokenRefreshed = "Token refreshed successfully.";
        public const string PasswordChanged = "Password changed successfully.";
    }
    #endregion

    #region Basket Service Success
    public static class Basket
    {
        public const string ItemAdded = "Item added to basket successfully.";
        public const string ItemUpdated = "Basket item updated successfully.";
        public const string ItemRemoved = "Item removed from basket successfully.";
        public const string Cleared = "Basket cleared successfully.";
        public const string CheckedOut = "Basket checked out successfully.";
    }
    #endregion

    #region Order Service Success
    public static class Order
    {
        public const string Created = "Order created successfully.";
        public const string StatusUpdated = "Order status updated successfully.";
        public const string Cancelled = "Order cancelled successfully.";
    }
    #endregion

    #region Payment Service Success
    public static class Payment
    {
        public const string Success = "Payment processed successfully.";
        public const string Refunded = "Payment refunded successfully.";
    }
    #endregion
}
