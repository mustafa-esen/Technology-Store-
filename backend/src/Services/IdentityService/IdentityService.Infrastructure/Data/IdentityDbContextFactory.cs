using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IdentityService.Infrastructure.Data;

public class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost,1451;Database=IdentityDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True");

        return new IdentityDbContext(optionsBuilder.Options);
    }
}
