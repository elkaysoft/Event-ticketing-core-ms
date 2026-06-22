using ETS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETS.Infrastructure.Persistence.Configuration
{
    public class VerificationTokenConfiguration : IEntityTypeConfiguration<VerificationToken>
    {
        public void Configure(EntityTypeBuilder<VerificationToken> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.CreatedBy).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Token).HasMaxLength(500).IsRequired();
            builder.Property(x => x.RequestId).HasMaxLength(100);
        }
    }
}
