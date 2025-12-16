using Microsoft.EntityFrameworkCore;
using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Data;

namespace IdentityService.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly IdentityDbContext _context;

    public RoleRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.Name == name);
    }

    public async Task<Role?> GetByIdAsync(Guid id)
    {
        return await _context.Roles.FindAsync(id);
    }

    public async Task AddUserToRoleAsync(Guid userId, Guid roleId)
    {
        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = roleId,
            AssignedAt = DateTime.UtcNow
        };

        await _context.UserRoles.AddAsync(userRole);
        await _context.SaveChangesAsync();
    }
}
