using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Data;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasIndex(e => e.Name);

            entity.HasData(
                new Category { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Laptop", Description = "Dizüstü Bilgisayarlar", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Category { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Telefon", Description = "Akıllı Telefonlar", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Category { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Tablet", Description = "Tablet Bilgisayarlar", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Category { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Aksesuar", Description = "Teknoloji Aksesuarları", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Brand).HasMaxLength(100);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);

            entity.HasOne(e => e.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.CategoryId);
            entity.HasIndex(e => e.Brand);
            entity.HasIndex(e => e.IsActive);
        });
    }
}
