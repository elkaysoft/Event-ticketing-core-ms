using ETS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETS.Infrastructure.Persistence.Configuration
{
    public class EventsConfiguration : IEntityTypeConfiguration<Events>
    {
        public void Configure(EntityTypeBuilder<Events> builder)
        {
            builder.ToTable("Events");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).IsRequired().HasMaxLength(500);
            builder.Property(x => x.Description).HasMaxLength(5000);
            builder.Property(x => x.BannerUrl).HasMaxLength(500);
            builder.Property(x => x.KickoffTime).IsRequired().HasMaxLength(15);
            builder.Property(x => x.Location).IsRequired().HasMaxLength(500);

            
        }
    }
}
