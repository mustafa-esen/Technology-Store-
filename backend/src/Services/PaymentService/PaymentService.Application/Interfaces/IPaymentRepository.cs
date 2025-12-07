using PaymentService.Domain.Entities;

namespace PaymentService.Application.Interfaces;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(Guid id);
    Task<Payment?> GetByOrderIdAsync(string orderId);
    Task<List<Payment>> GetByUserIdAsync(string userId);
    Task<List<Payment>> GetAllAsync();
    Task<Payment> AddAsync(Payment payment);
    Task UpdateAsync(Payment payment);
    Task<bool> OrderAlreadyProcessedAsync(string orderId);
}
