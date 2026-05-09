using Framework.Core.Application.Commands;
using Framework.Core.Domain.Repositories;
using Framework.Core.Domain.Services;
using Framework.Infrastructure.Queries;
using Framework.Infrastructure.Tools.Clocks;
using Framework.Infrastructure.Tools.IdGenerators;
using Framework.Infrastructure.Tools.JsonSerializers.NewtonSoft;
using Framework.Presentation.AspNetCore.Decorators;
using Framework.Presentation.AspNetCore.Services;
using IdGen;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;
using FluentValidation;
using Framework.Infrastructure.Tools.Validators.FluentValidation;
using Framework.Presentation.AspNetCore.Resolvers;
using Microsoft.AspNetCore.Identity;


namespace Framework.Presentation.AspNetCore.Extensions;

public static class ServiceCollectionExtensions
{
   
    public static IServiceCollection AddHttpContextServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.TryAddSingleton<IIdentityService, IdentityService>();

        return services;
    }
    
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {

        services.AddSingleton<IClock, UtcClock>();

        services.AddTransient<ICommandBus, CommandBus>();

        services.AddTransient<IQueryBus, QueryBus>();

        services.AddSingleton(_ => new IdGenerator(0));

        services.AddSingleton<IIdGenerator,SnowflakeIdGenerator>();

        services.AddSingleton<IJsonSerializerAdapter, NewtonSoftSerializerAdapter>();

        return services;
    }

    public static IServiceCollection AddQueryHandlers(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.Scan(s => s.FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableToAny(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.TryDecorate(typeof(IQueryHandler<,>), typeof(QueryHandlerLogDecorator<,>));

        return services;
    }

    public static IServiceCollection AddCommandHandlers(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.Scan(s => s.FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableToAny(typeof(ICommandHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.TryDecorate(typeof(ICommandHandler<>), typeof(CommandHandlerLogDecorator<>));

        services.Scan(s => s.FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableToAny(typeof(ICommandHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.TryDecorate(typeof(ICommandHandler<,>), typeof(CommandHandlerLogDecorator<,>));

        return services;
    }

    public static IServiceCollection AddDomainServices(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.Scan(s => s.FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableToAny(typeof(IDomainService)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.Scan(s => s.FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableToAny(typeof(IRepository<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }

    public static IServiceCollection AddQueryRepositories(this IServiceCollection services,params Assembly[] assemblies)
    {
        services.Scan(s => s.FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableToAny(typeof(IQueryRepository)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }

    public static IServiceCollection AddValidators(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddValidatorsFromAssemblies(assemblies);
        
        services.AddSingleton<IValidatorAdapter, FluentValidationAdapter>();

        return services;
    }

}
