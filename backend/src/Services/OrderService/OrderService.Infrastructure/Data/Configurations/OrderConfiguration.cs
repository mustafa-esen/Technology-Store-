using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using OrderService.Domain.ValueObjects;

namespace OrderService.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .ValueGeneratedNever();

        builder.Property(o => o.UserId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<OrderStatus>(v));

        builder.Property(o => o.TotalAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.Notes)
            .HasMaxLength(500);

        builder.Property(o => o.PaymentIntentId)
            .HasMaxLength(200);

        builder.Property(o => o.PaymentMethod)
            .HasMaxLength(50);

        builder.Property(o => o.CreatedDate)
            .IsRequired();

        builder.Property(o => o.UpdatedDate)
            .IsRequired(false);

        // Configure Address as owned entity
        builder.OwnsOne(o => o.ShippingAddress, address =>
        {
            address.Property(a => a.Street).IsRequired().HasMaxLength(200);
            address.Property(a => a.City).IsRequired().HasMaxLength(100);
            address.Property(a => a.State).IsRequired().HasMaxLength(100);
            address.Property(a => a.ZipCode).IsRequired().HasMaxLength(20);
            address.Property(a => a.Country).IsRequired().HasMaxLength(100);
        });

        // Configure relationship with OrderItems
        builder.HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(o => o.UserId);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.CreatedDate);
    }
}
