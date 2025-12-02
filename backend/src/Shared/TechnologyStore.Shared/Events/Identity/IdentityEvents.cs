namespace TechnologyStore.Shared.Events.Identity;

/// Kullanıcı kayıt olduğunda yayınlanan event
/// Publisher: IdentityService
/// Consumer: NotificationService
public interface IUserRegisteredEvent
{
    string UserId { get; set; }
    string Email { get; set; }
    string FullName { get; set; }
    DateTime RegisteredDate { get; set; }
}

/// Kullanıcı giriş yaptığında yayınlanan event (Opsiyonel - Analytics için)
/// Publisher: IdentityService
/// Consumer: NotificationService
public interface IUserLoggedInEvent
{
    string UserId { get; set; }
    string Email { get; set; }
    DateTime LoggedInDate { get; set; }
    string IpAddress { get; set; }
}
