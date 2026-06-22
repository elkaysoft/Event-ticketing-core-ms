using ETS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETS.Infrastructure.Persistence.Configuration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Domain.Entities.Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.EventName).IsRequired().HasMaxLength(500);
            builder.Property(x => x.FullName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.EmailAddress).IsRequired().HasMaxLength(100);
            builder.Property(x => x.PhoneNumber).IsRequired().HasMaxLength(20);
            builder.Property(x => x.OrderNumber).IsRequired().HasMaxLength(50);
            builder.Property(x => x.PaystackAccessCode).IsRequired().HasMaxLength(500);
            builder.Property(x => x.OrderStatus).IsRequired().HasConversion<string>();
            builder.Property(x => x.RedemptionStatus).HasConversion<string>();
            builder.Property(x => x.TotalAmount).IsRequired().HasPrecision(10, 2);
            builder.Property(x => x.SubTotal).IsRequired().HasPrecision(10, 2);
            builder.Property(x => x.TaxAmount).IsRequired().HasPrecision(5, 2);

            builder.HasMany(x => x.OrderItems)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
