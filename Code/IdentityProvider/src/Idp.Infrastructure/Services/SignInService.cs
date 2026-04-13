using Idp.Domain._Shared.Contracts;
using Idp.Domain.UserAggregate;
using Idp.Infrastructure.Persistence.UserAggregate;
using Microsoft.AspNetCore.Identity;

namespace Idp.Infrastructure.Services;

public class SignInService(
    UserManager<UserDataModel> userManager,
    SignInManager<UserDataModel> signInManager):ISignInService
{
    public async Task<bool> ValidatePasswordAsync(User user, string password, CancellationToken cancellationToken = default)
    {
        var userDataModel = await userManager.FindByIdAsync(user.Id.ToString());

        if (userDataModel is null)
            return false;

        return await userManager.CheckPasswordAsync(userDataModel, password);
    }

    public async Task SignInAsync(User user, CancellationToken cancellationToken = default)
    {
       var userDataModel= await userManager.FindByIdAsync(user.Id.ToString());

       await signInManager.SignInAsync(userDataModel!,true);
    }

    public async Task SignOutAsync(CancellationToken cancellationToken = default)
    {
       await signInManager.SignOutAsync();
    }
}
