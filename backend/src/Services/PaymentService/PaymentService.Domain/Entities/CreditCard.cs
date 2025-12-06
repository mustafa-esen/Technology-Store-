namespace PaymentService.Domain.Entities;

public class CreditCard
{
    public Guid Id { get; private set; }
    public string UserId { get; private set; }
    public string CardHolderName { get; private set; }
    public string CardNumber { get; private set; }
    public string ExpiryMonth { get; private set; }
    public string ExpiryYear { get; private set; }
    public string CardType { get; private set; }
    public bool IsDefault { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime? UpdatedDate { get; private set; }

    private CreditCard() { }

    public CreditCard(
        string userId,
        string cardHolderName,
        string cardNumber,
        string expiryMonth,
        string expiryYear)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId cannot be empty", nameof(userId));

        if (string.IsNullOrWhiteSpace(cardHolderName))
            throw new ArgumentException("Card holder name cannot be empty", nameof(cardHolderName));

        if (string.IsNullOrWhiteSpace(cardNumber))
            throw new ArgumentException("Card number cannot be empty", nameof(cardNumber));

        ValidateExpiryDate(expiryMonth, expiryYear);

        Id = Guid.NewGuid();
        UserId = userId;
        CardHolderName = cardHolderName;
        CardNumber = MaskCardNumber(cardNumber);
        ExpiryMonth = expiryMonth;
        ExpiryYear = expiryYear;
        CardType = DetectCardType(cardNumber);
        IsDefault = false;
        CreatedDate = DateTime.UtcNow;
    }

    public void SetAsDefault()
    {
        IsDefault = true;
        UpdatedDate = DateTime.UtcNow;
    }

    public void RemoveDefault()
    {
        IsDefault = false;
        UpdatedDate = DateTime.UtcNow;
    }

    public void Update(string cardHolderName, string expiryMonth, string expiryYear)
    {
        if (string.IsNullOrWhiteSpace(cardHolderName))
            throw new ArgumentException("Card holder name cannot be empty", nameof(cardHolderName));

        ValidateExpiryDate(expiryMonth, expiryYear);

        CardHolderName = cardHolderName;
        ExpiryMonth = expiryMonth;
        ExpiryYear = expiryYear;
        UpdatedDate = DateTime.UtcNow;
    }

    private static string MaskCardNumber(string cardNumber)
    {
        var digitsOnly = new string(cardNumber.Where(char.IsDigit).ToArray());

        if (digitsOnly.Length < 4)
            throw new ArgumentException("Card number must have at least 4 digits", nameof(cardNumber));

        var lastFour = digitsOnly.Substring(digitsOnly.Length - 4);
        return $"**** **** **** {lastFour}";
    }

    private static string DetectCardType(string cardNumber)
    {
        var digitsOnly = new string(cardNumber.Where(char.IsDigit).ToArray());

        if (digitsOnly.StartsWith("4"))
            return "Visa";
        else if (digitsOnly.StartsWith("5"))
            return "MasterCard";
        else if (digitsOnly.StartsWith("3"))
            return "American Express";
        else
            return "Unknown";
    }

    private static void ValidateExpiryDate(string month, string year)
    {
        if (!int.TryParse(month, out int m) || m < 1 || m > 12)
            throw new ArgumentException("Invalid expiry month. Must be between 01 and 12", nameof(month));

        if (!int.TryParse(year, out int y) || y < 0 || y > 99)
            throw new ArgumentException("Invalid expiry year. Must be between 00 and 99", nameof(year));

        var currentYear = DateTime.UtcNow.Year % 100;
        var currentMonth = DateTime.UtcNow.Month;

        if (y < currentYear || (y == currentYear && m < currentMonth))
            throw new ArgumentException("Card has expired", nameof(year));
    }
}
