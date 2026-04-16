using ETS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETS.Infrastructure.Persistence.Configuration
{
    public class EventCategoriesConfiguration : IEntityTypeConfiguration<EventCategory>
    {
        public void Configure(EntityTypeBuilder<EventCategory> builder)
        {
            builder.ToTable("EventCategory");
            builder.HasKey(e => e.Id);
            builder.Property(x => x.Title).HasMaxLength(500).IsRequired();
            builder.Property(x => x.Price).HasPrecision(8, 2);

            builder.HasOne(s => s.Event)
                .WithMany(s => s.EventCategories)
                .HasForeignKey(s => s.EventId)
                .OnDelete(DeleteBehavior.Restrict);
            
        }
    }
}
