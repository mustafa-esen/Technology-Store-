using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Entities;
using PaymentService.Infrastructure.Data;

namespace PaymentService.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly PaymentDbContext _context;
    private readonly ILogger<PaymentRepository> _logger;

    public PaymentRepository(PaymentDbContext context, ILogger<PaymentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Payment?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("§Y'ó Fetching payment by Id: {PaymentId}", id);
        return await _context.Payments.FindAsync(id);
    }

    public async Task<Payment?> GetByOrderIdAsync(string orderId)
    {
        _logger.LogInformation("§Y'ó Fetching payment by OrderId: {OrderId}", orderId);
        return await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
    }

    public async Task<List<Payment>> GetByUserIdAsync(string userId)
    {
        _logger.LogInformation("§Y'ó Fetching payments for UserId: {UserId}", userId);
        return await _context.Payments
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedDate)
            .ToListAsync();
    }

    public async Task<List<Payment>> GetAllAsync()
    {
        _logger.LogInformation("§Y'ó Fetching all payments (admin)");
        return await _context.Payments
            .OrderByDescending(p => p.CreatedDate)
            .ToListAsync();
    }

    public async Task<Payment> AddAsync(Payment payment)
    {
        _logger.LogInformation("§Y'ó Adding new payment for OrderId: {OrderId}", payment.OrderId);
        await _context.Payments.AddAsync(payment);
        await _context.SaveChangesAsync();
        return payment;
    }

    public async Task UpdateAsync(Payment payment)
    {
        _logger.LogInformation("§Y'ó Updating payment Id: {PaymentId}, Status: {Status}", payment.Id, payment.Status);
        _context.Payments.Update(payment);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> OrderAlreadyProcessedAsync(string orderId)
    {
        return await _context.Payments.AnyAsync(p => p.OrderId == orderId);
    }
}
