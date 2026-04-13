using Framework.Infrastructure.EventProcessor.EventBus.MassTransit;
using Framework.Presentation.AspNetCore.Extensions;
using Idp.Application.UserAggregate.RequestSmsOtp;
using Idp.Infrastructure.Configurations;
using Idp.Infrastructure.Persistence.UserAggregate;
using Idp.Infrastructure.Persistence.UserOtpSecretAggregate;
using Idp.Infrastructure.Services;
using Idp.Presentation._Shared.HostedServices;
using Idp.Presentation.UserAggregate.Consumers.WhenSmsOtpRequested;
using MassTransit;


namespace Idp.ServiceHost;

public static class Configurations
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var commandsAssemblies = new[] { typeof(RequestSmsOtpCommand).Assembly };
        var domainServicesAssemblies = new[] { typeof(OtpNetService).Assembly };
        var repositoriesAssemblies = new[] { typeof(UserOtpSecretRepository).Assembly, typeof(UserPhoneNumberResolver).Assembly };




        builder.Services.AddControllers();
        builder.Services.AddRazorPages();
        builder.Services
            .AddSwaggerGen()
            .AddHttpContextServices()
            .AddCoreServices()
            .AddDomainServices(domainServicesAssemblies)
            .AddCommandHandlers(commandsAssemblies)
            .AddRepositories(repositoriesAssemblies)
            .AddQueryRepositories(repositoriesAssemblies)
            .AddPersistence(builder.Configuration)
            .AddEventPublisher(builder.Configuration)
            .AddMessageInbox(builder.Configuration)
            .AddMassTransitConsumers(builder.Configuration)
            .UseOpenIddict(builder.Configuration);
        builder.Services.AddHostedService<ClientsSeeder>();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseStaticFiles();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.MapRazorPages();


        return app;
    }



    private static IServiceCollection AddMassTransitConsumers(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(configurator =>
        {
            ArgumentException.ThrowIfNullOrEmpty(configuration["Infrastructures:RabbitMq:UserName"]);
            ArgumentException.ThrowIfNullOrEmpty(configuration["Infrastructures:RabbitMq:Password"]);

            configurator.AddConsumer<SendSmsWhenSmsOtpRequested>();

            configurator.UsingRabbitMq((context, config) =>
            {

                config.UseMessageRetry(x => x.Immediate(3));

                config.Host(configuration["Infrastructures:RabbitMq:Server"], "/", h =>
                {
                    h.Username(configuration["Infrastructures:RabbitMq:UserName"]!);
                    h.Password(configuration["Infrastructures:RabbitMq:Password"]!);

                });

                config.ReceiveEndpoint("Temaj.Internal.Events", e =>
                {


                    e.PrefetchCount = 0;
                    e.ConcurrentMessageLimit = 1;
                    e.ConfigureConsumer<SendSmsWhenSmsOtpRequested>(context);
                });

               
            });
        });

        services.AddHostedService<MassTransitConsoleHostedService>();

        return services;
    }   

   

}