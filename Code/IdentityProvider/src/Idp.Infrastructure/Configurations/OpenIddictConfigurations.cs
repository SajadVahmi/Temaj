using Idp.Infrastructure.Persistence._Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Validation.AspNetCore;

namespace Idp.Infrastructure.Configurations;
using static OpenIddict.Abstractions.OpenIddictConstants;

public static class OpenIddictConfigurations
{
    public static IServiceCollection UseOpenIddict(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
        });

        services.AddOpenIddict()
            .AddCore(o => o.UseEntityFrameworkCore().UseDbContext<IdpDbContext>())
            .AddServer(o =>
            {
                o.SetAuthorizationEndpointUris("/connect/authorize")
                    .SetTokenEndpointUris("/connect/token")
                    .SetUserInfoEndpointUris("/connect/userinfo")
                    .SetIntrospectionEndpointUris("/connect/introspect");

                o.AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange();
                o.AllowClientCredentialsFlow();
                o.AllowRefreshTokenFlow();
                o.AllowTokenExchangeFlow();

                o.RegisterScopes(Scopes.OpenId, Scopes.Profile, Scopes.Email, "offline_access");


                o.AddDevelopmentEncryptionCertificate()
                    .AddDevelopmentSigningCertificate();

                o.DisableAccessTokenEncryption();
                o.UseAspNetCore();


            })
            .AddValidation(options =>
            {
                options.UseLocalServer();
                options.UseAspNetCore();
            });

        return services;
    }
}
