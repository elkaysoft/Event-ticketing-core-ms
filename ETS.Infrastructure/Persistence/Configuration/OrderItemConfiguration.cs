using ETS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETS.Infrastructure.Persistence.Configuration
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<Domain.Entities.OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Unit).IsRequired();
            builder.Property(x => x.UnitPrice).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.RedemptionStatus).HasConversion<string>();
            builder.Property(x => x.TicketGenerationStatus).HasConversion<string>();
            builder.Property(x => x.PaymentStatus).HasConversion<string>();
            builder.Property(x => x.QRCodeReference).HasMaxLength(250);
            builder.Property(x => x.QRCodeUrl).HasMaxLength(500);

            builder.Property(x => x.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            builder.HasOne(x => x.EventCategory)
                .WithMany()
                .HasForeignKey(x => x.EventCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }        
    }
}