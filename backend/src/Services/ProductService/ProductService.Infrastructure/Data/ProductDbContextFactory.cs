using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProductService.Infrastructure.Data;

public class ProductDbContextFactory : IDesignTimeDbContextFactory<ProductDbContext>
{
    public ProductDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ProductDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=localhost,1450;Database=ProductDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True",
            b => b.MigrationsAssembly("ProductService.Infrastructure")
        );

        return new ProductDbContext(optionsBuilder.Options);
    }
}
