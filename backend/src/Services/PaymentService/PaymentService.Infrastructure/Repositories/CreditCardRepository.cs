using Microsoft.EntityFrameworkCore;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Entities;
using PaymentService.Infrastructure.Data;

namespace PaymentService.Infrastructure.Repositories;

public class CreditCardRepository : ICreditCardRepository
{
    private readonly PaymentDbContext _context;

    public CreditCardRepository(PaymentDbContext context)
    {
        _context = context;
    }

    public async Task<CreditCard?> GetByIdAsync(Guid id)
    {
        return await _context.CreditCards
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<CreditCard>> GetByUserIdAsync(string userId)
    {
        return await _context.CreditCards
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.IsDefault)
            .ThenByDescending(c => c.CreatedDate)
            .ToListAsync();
    }

    public async Task<CreditCard?> GetDefaultCardByUserIdAsync(string userId)
    {
        return await _context.CreditCards
            .FirstOrDefaultAsync(c => c.UserId == userId && c.IsDefault);
    }

    public async Task<CreditCard> AddAsync(CreditCard creditCard)
    {
        await _context.CreditCards.AddAsync(creditCard);
        await _context.SaveChangesAsync();
        return creditCard;
    }

    public async Task<CreditCard> UpdateAsync(CreditCard creditCard)
    {
        _context.CreditCards.Update(creditCard);
        await _context.SaveChangesAsync();
        return creditCard;
    }

    public async Task DeleteAsync(Guid id)
    {
        var creditCard = await GetByIdAsync(id);
        if (creditCard != null)
        {
            _context.CreditCards.Remove(creditCard);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.CreditCards.AnyAsync(c => c.Id == id);
    }
}
