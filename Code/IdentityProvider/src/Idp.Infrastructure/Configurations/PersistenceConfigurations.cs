using Framework.Core.Domain.Repositories;
using Idp.Infrastructure.Persistence._Shared;
using Idp.Infrastructure.Persistence.RoleAggregate;
using Idp.Infrastructure.Persistence.UserAggregate;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Idp.Infrastructure.Configurations;

public static class PersistenceConfigurations
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {


        services.AddScoped(_ => CreateDbContext(services, configuration));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddIdentity<UserDataModel, RoleDataModel>(opt =>
            {
                opt.SignIn.RequireConfirmedAccount = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireDigit = false;
            })
            .AddEntityFrameworkStores<IdpDbContext>()
            .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/RequestOtpCode/Index";
            options.AccessDeniedPath = "/RequestOtpCode/Index";
        });


        return services;

    }

    private static IdpDbContext CreateDbContext(IServiceCollection services,
        IConfiguration configuration)
    {
        var commandDbContextConnectionString =
            configuration.GetSection("Infrastructures:SqlServer").Get<string>();

        ArgumentNullException.ThrowIfNull(commandDbContextConnectionString);

        var options =
            new DbContextOptionsBuilder<IdpDbContext>()
                .UseSqlServer(commandDbContextConnectionString)
                .UseOpenIddict()
                .UseApplicationServiceProvider(services.BuildServiceProvider())
                .Options;

        var dbContext = new IdpDbContext(options,true);

        return dbContext;

    }



}
