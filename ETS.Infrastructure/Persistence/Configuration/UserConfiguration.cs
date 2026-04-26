using ETS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETS.Infrastructure.Persistence.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Role).HasConversion<int>().IsRequired();
            builder.Property(x => x.EmailAddress).HasMaxLength(150).IsRequired();
            builder.Property(x => x.FullName).HasMaxLength(500).IsRequired();
            builder.Property(x => x.PhoneNumber).HasMaxLength(20);
            builder.Property(x => x.Password).HasMaxLength(500).IsRequired();
            builder.Property(x => x.Status).HasConversion<int>().IsRequired().IsRequired();
            
            

        }
    }
}
