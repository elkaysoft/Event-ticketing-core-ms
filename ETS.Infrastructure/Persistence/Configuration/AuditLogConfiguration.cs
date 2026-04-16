using ETS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETS.Infrastructure.Persistence.Configuration
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.TableName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.ColumnName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.KeyValue)
                .HasMaxLength(100);

            builder.Property(x => x.OldValue)
                .HasMaxLength(500);

            builder.Property(x => x.NewValue)
                .HasMaxLength(500);

            builder.Property(x => x.ChangedAt)
                .IsRequired();

            builder.Property(x => x.ChangedBy)
                .HasMaxLength(200);

            builder.Property(x => x.EventType)
                .IsRequired()
                .HasMaxLength(50);

            // Index for common queries
            builder.HasIndex(x => x.TableName);
            builder.HasIndex(x => x.ChangedAt);
            builder.HasIndex(x => x.ChangedBy);
        }
    }
}
