namespace TechnologyStore.Shared.Constants;

/// <summary>
/// API route sabitleri - Gateway ve mikroservisler arasında tutarlılık sağlar
/// </summary>
public static class ApiRoutes
{
    public const string ApiPrefix = "api";
    public const string ApiVersion = "v1";

    #region Product Service Routes
    public static class Products
    {
        public const string Base = $"{ApiPrefix}/{ApiVersion}/products";
        public const string GetAll = Base;
        public const string GetById = $"{Base}/{{id}}";
        public const string GetByCategory = $"{Base}/category/{{categoryId}}";
        public const string Create = Base;
        public const string Update = $"{Base}/{{id}}";
        public const string Delete = $"{Base}/{{id}}";
    }

    public static class Categories
    {
        public const string Base = $"{ApiPrefix}/{ApiVersion}/categories";
        public const string GetAll = Base;
        public const string GetById = $"{Base}/{{id}}";
        public const string Create = Base;
        public const string Update = $"{Base}/{{id}}";
        public const string Delete = $"{Base}/{{id}}";
    }
    #endregion

    #region Identity Service Routes
    public static class Identity
    {
        public const string Base = $"{ApiPrefix}/{ApiVersion}/identity";
        public const string Register = $"{Base}/register";
        public const string Login = $"{Base}/login";
        public const string RefreshToken = $"{Base}/refresh-token";
        public const string Logout = $"{Base}/logout";
        public const string ChangePassword = $"{Base}/change-password";
    }
    #endregion

    #region Basket Service Routes
    public static class Basket
    {
        public const string Base = $"{ApiPrefix}/{ApiVersion}/basket";
        public const string Get = $"{Base}/{{userId}}";
        public const string AddItem = $"{Base}/items";
        public const string UpdateItem = $"{Base}/items/{{productId}}";
        public const string RemoveItem = $"{Base}/items/{{productId}}";
        public const string Clear = $"{Base}/{{userId}}";
        public const string Checkout = $"{Base}/checkout";
    }
    #endregion

    #region Order Service Routes
    public static class Orders
    {
        public const string Base = $"{ApiPrefix}/{ApiVersion}/orders";
        public const string GetById = $"{Base}/{{orderId}}";
        public const string GetUserOrders = $"{Base}/user/{{userId}}";
        public const string Create = Base;
        public const string UpdateStatus = $"{Base}/{{orderId}}/status";
        public const string Cancel = $"{Base}/{{orderId}}/cancel";
    }
    #endregion

    #region Payment Service Routes
    public static class Payment
    {
        public const string Base = $"{ApiPrefix}/{ApiVersion}/payment";
        public const string CreateIntent = $"{Base}/create-intent";
        public const string ConfirmPayment = $"{Base}/confirm";
        public const string RefundPayment = $"{Base}/refund/{{orderId}}";
        public const string GetPaymentStatus = $"{Base}/status/{{orderId}}";
    }
    #endregion

    #region Notification Service Routes
    public static class Notification
    {
        public const string Base = $"{ApiPrefix}/{ApiVersion}/notification";
        public const string SendEmail = $"{Base}/email";
        public const string SendSms = $"{Base}/sms";
        public const string GetNotifications = $"{Base}/user/{{userId}}";
    }
    #endregion
}
