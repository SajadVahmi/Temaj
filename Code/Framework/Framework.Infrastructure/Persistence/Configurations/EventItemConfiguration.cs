using AeroTech.Framework.Infrastructure.Persistence.Constants;
using Framework.Infrastructure.Persistence.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Framework.Infrastructure.Persistence.Configurations;

public class EventItemConfiguration : IEntityTypeConfiguration<EventItem>
{
    public void Configure(EntityTypeBuilder<EventItem> builder)
    {

        builder.ToTable(PersistenceConstant.DomainEvent);

        builder.HasKey(c => c.Id);

        builder.Property(c => c.OccurredByUserId).HasMaxLength(255);

        builder.Property(c => c.EventName).HasMaxLength(255);

        builder.Property(c => c.AggregateName).HasMaxLength(255);

        builder.Property(c => c.EventTypeName).HasMaxLength(500);

        builder.Property(c => c.AggregateTypeName).HasMaxLength(500);
    }
}


