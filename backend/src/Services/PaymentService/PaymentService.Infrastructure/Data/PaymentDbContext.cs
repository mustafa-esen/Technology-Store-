using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Entities;

namespace PaymentService.Infrastructure.Data;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
    {
    }

    public DbSet<Payment> Payments { get; set; }
    public DbSet<CreditCard> CreditCards { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.OrderId)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(e => e.OrderId)
                .IsUnique();

            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(e => e.UserId);

            entity.OwnsOne(e => e.Amount, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("Amount")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                money.Property(m => m.Currency)
                    .HasColumnName("Currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(e => e.TransactionId)
                .HasMaxLength(100);

            entity.Property(e => e.FailureReason)
                .HasMaxLength(500);

            entity.Property(e => e.CreatedDate)
                .IsRequired();

            entity.Property(e => e.ProcessedDate);
        });

        modelBuilder.Entity<CreditCard>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(e => e.UserId);

            entity.Property(e => e.CardHolderName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.CardNumber)
                .IsRequired()
                .HasMaxLength(19);

            entity.Property(e => e.ExpiryMonth)
                .IsRequired()
                .HasMaxLength(2);

            entity.Property(e => e.ExpiryYear)
                .IsRequired()
                .HasMaxLength(2);

            entity.Property(e => e.CardType)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.IsDefault)
                .IsRequired();

            entity.HasIndex(e => new { e.UserId, e.IsDefault });

            entity.Property(e => e.CreatedDate)
                .IsRequired();

            entity.Property(e => e.UpdatedDate);
        });
    }
}
