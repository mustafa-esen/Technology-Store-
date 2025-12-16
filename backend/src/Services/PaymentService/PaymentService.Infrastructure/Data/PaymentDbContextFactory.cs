using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PaymentService.Infrastructure.Data;

public class PaymentDbContextFactory : IDesignTimeDbContextFactory<PaymentDbContext>
{
    public PaymentDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PaymentDbContext>();

        optionsBuilder.UseSqlServer("Server=localhost,1450;Database=PaymentServiceDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;");

        return new PaymentDbContext(optionsBuilder.Options);
    }
}
