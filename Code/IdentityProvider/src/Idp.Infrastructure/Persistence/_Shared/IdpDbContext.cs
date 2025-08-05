using Framework.Core.Domain.Services;
using Framework.Infrastructure.Persistence.Configurations;
using Framework.Infrastructure.Persistence.Events;
using Framework.Infrastructure.Persistence.Extensions;
using Framework.Infrastructure.Persistence.Helpers;
using Idp.Infrastructure.Persistence.RoleAggregate;
using Idp.Infrastructure.Persistence.UserAggregate;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Idp.Infrastructure.Persistence._Shared;

public class IdpDbContext : IdentityDbContext<UserDataModel,RoleDataModel,long>
{
  
    public IdpDbContext(DbContextOptions options, bool saveDomainEvents = false) : base(options)
    {
        SaveDomainEvents = saveDomainEvents;
        Sequence = new SequenceHelper(this);
    }

    public SequenceHelper Sequence { get; private set; }
    protected bool SaveDomainEvents { get; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (SaveDomainEvents)

            modelBuilder.ApplyConfiguration(new EventItemConfiguration());

        else

            modelBuilder.Ignore<EventItem>();

        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        base.ConfigureConventions(builder);

    }

    public override int SaveChanges()
    {

        BeforeSaveTriggers();

        var result = base.SaveChanges();

        return result;
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {

        BeforeSaveTriggers();

        var result = base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

        return result;
    }


    private void BeforeSaveTriggers()
    {
        ChangeTracker.DetectChanges();

        var identityService = this.GetService<IIdentityService>();

        var serializer = this.GetService<IJsonSerializerAdapter>();

        if (SaveDomainEvents)
            PersistDomainEventsInOutBoxEventTable(identityService, serializer);

    }



    private void PersistDomainEventsInOutBoxEventTable(IIdentityService identityService,
        IJsonSerializerAdapter jsonSerializer)
    {

        var hasAnyEventAggregate = ChangeTracker.GetAggregatesWithEvent();

        var domainEvents = hasAnyEventAggregate
            .SelectMany(aggregateRoot =>
                EventItemFactory.Create(aggregateRoot, aggregateRoot.GetEvents(), jsonSerializer, identityService))
            .ToList();

        Set<EventItem>().AddRange(domainEvents);

        hasAnyEventAggregate.ForEach(a => a.ClearEvents());
    }
}