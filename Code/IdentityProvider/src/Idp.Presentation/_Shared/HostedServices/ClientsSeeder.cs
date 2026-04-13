using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Idp.Presentation._Shared.HostedServices;

public class ClientsSeeder(IServiceProvider provider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = provider.CreateScope();
        var mgr = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        
        if (await mgr.FindByClientIdAsync("postman-test-client", cancellationToken) == null)
        {
            await mgr.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "postman-test-client",
                DisplayName = "PostMan Test Client",
                RedirectUris = { new Uri("http://localhost:3000/callback") },
                PostLogoutRedirectUris = { new Uri("http://localhost:3000/") },
                Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.ResponseTypes.Code,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Profile,
                    Permissions.GrantTypes.RefreshToken
                },
                Requirements =
                {
                    Requirements.Features.ProofKeyForCodeExchange 
                }
            }, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}