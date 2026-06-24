using ETS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETS.Infrastructure.Persistence.Configuration
{
    public class EmailLogConfiguration : IEntityTypeConfiguration<EmailLog>
    {
        public void Configure(EntityTypeBuilder<EmailLog> builder)
        {
            builder.ToTable("EmailLogs");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Sender).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Recipient).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Subject).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Body).IsRequired();
            builder.Property(x => x.NotificationStatus).HasConversion<string>();
            builder.Property(x => x.NotificationTarget).HasConversion<string>();

        }
    }
}
