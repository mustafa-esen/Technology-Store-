using PaymentService.Domain.Entities;

namespace PaymentService.Application.Interfaces;

public interface ICreditCardRepository
{
    Task<CreditCard?> GetByIdAsync(Guid id);
    Task<List<CreditCard>> GetByUserIdAsync(string userId);
    Task<CreditCard?> GetDefaultCardByUserIdAsync(string userId);
    Task<CreditCard> AddAsync(CreditCard creditCard);
    Task<CreditCard> UpdateAsync(CreditCard creditCard);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
