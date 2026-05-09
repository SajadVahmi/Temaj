using Idp.Domain._Shared.Contracts;
using Idp.Domain._Shared.Enums;
using Idp.Domain.UserAggregate;
using Idp.Infrastructure.Persistence.UserAggregate;
using Microsoft.AspNetCore.Identity;

namespace Idp.Infrastructure.Services;

public class SignInService(
    UserManager<UserDataModel> userManager,
    SignInManager<UserDataModel> signInManager) : ISignInService
{
    public async Task<bool> ValidatePasswordAsync(User user, string password, CancellationToken cancellationToken = default)
    {
        var userDataModel = await userManager.FindByIdAsync(user.Id.ToString());

        if (userDataModel is null)
            return false;

        return await userManager.CheckPasswordAsync(userDataModel, password);
    }

    public async Task<PasswordSignInStatus> SignInWithPasswordAsync(User user, string password, CancellationToken cancellationToken = default)
    {
        var userDataModel = await userManager.FindByIdAsync(user.Id.ToString());

        if (userDataModel is null || string.IsNullOrWhiteSpace(userDataModel.UserName))
            return PasswordSignInStatus.Failed;

        var signInResult = await signInManager.PasswordSignInAsync(userDataModel.UserName, password, true, false);

        if (signInResult.RequiresTwoFactor)
            return PasswordSignInStatus.RequiresTwoFactor;

        return signInResult.Succeeded
            ? PasswordSignInStatus.Succeeded
            : PasswordSignInStatus.Failed;
    }

    public async Task<TwoFactorSignInStatus> SignInWithAuthenticatorCodeAsync(string verificationCode, bool rememberMachine, CancellationToken cancellationToken = default)
    {
        var code = verificationCode.Replace(" ", string.Empty).Replace("-", string.Empty);

        var twoFactorUser = await signInManager.GetTwoFactorAuthenticationUserAsync();

        if (twoFactorUser is null)
            return TwoFactorSignInStatus.NotAvailable;

        var signInResult = await signInManager.TwoFactorAuthenticatorSignInAsync(code, true, rememberMachine);

        return signInResult.Succeeded
            ? TwoFactorSignInStatus.Succeeded
            : TwoFactorSignInStatus.Failed;
    }

    public async Task<bool> HasPendingTwoFactorSignInAsync(CancellationToken cancellationToken = default)
    {
        var twoFactorUser = await signInManager.GetTwoFactorAuthenticationUserAsync();
        return twoFactorUser is not null;
    }

    public async Task<AuthenticatorSetupInfo?> GetOrCreateAuthenticatorSetupInfoAsync(User user, string issuer, CancellationToken cancellationToken = default)
    {
        var userDataModel = await userManager.FindByIdAsync(user.Id.ToString());

        if (userDataModel is null)
            return null;

        var sharedKey = await userManager.GetAuthenticatorKeyAsync(userDataModel);

        if (string.IsNullOrWhiteSpace(sharedKey))
        {
            await userManager.ResetAuthenticatorKeyAsync(userDataModel);
            sharedKey = await userManager.GetAuthenticatorKeyAsync(userDataModel);
        }

        sharedKey ??= string.Empty;

        var accountName = userDataModel.Email ?? userDataModel.UserName ?? userDataModel.PhoneNumber ?? userDataModel.Id.ToString();
        var qrCodeUri = GenerateQrCodeUri(issuer, accountName, sharedKey);
        var isTwoFactorEnabled = await userManager.GetTwoFactorEnabledAsync(userDataModel);

        return new AuthenticatorSetupInfo(
            SharedKey: sharedKey,
            QrCodeUri: qrCodeUri,
            IsTwoFactorEnabled: isTwoFactorEnabled,
            AccountName: accountName);
    }

    public async Task<bool> EnableAuthenticatorTwoFactorAsync(User user, string verificationCode, CancellationToken cancellationToken = default)
    {
        var userDataModel = await userManager.FindByIdAsync(user.Id.ToString());

        if (userDataModel is null)
            return false;

        var code = verificationCode.Replace(" ", string.Empty).Replace("-", string.Empty);

        var isCodeValid = await userManager.VerifyTwoFactorTokenAsync(
            userDataModel,
            userManager.Options.Tokens.AuthenticatorTokenProvider,
            code);

        if (!isCodeValid)
            return false;

        var setResult = await userManager.SetTwoFactorEnabledAsync(userDataModel, true);

        if (!setResult.Succeeded)
            return false;

        userDataModel.AuthenticatorTwoFactorEnabledAt = DateTimeOffset.UtcNow;
        var updateResult = await userManager.UpdateAsync(userDataModel);

        return updateResult.Succeeded;
    }

    public async Task DisableAuthenticatorTwoFactorAsync(User user, CancellationToken cancellationToken = default)
    {
        var userDataModel = await userManager.FindByIdAsync(user.Id.ToString());

        if (userDataModel is null)
            return;

        await userManager.SetTwoFactorEnabledAsync(userDataModel, false);
        userDataModel.AuthenticatorTwoFactorEnabledAt = null;
        await userManager.UpdateAsync(userDataModel);
        await signInManager.ForgetTwoFactorClientAsync();
    }

    public async Task<bool> IsAuthenticatorTwoFactorEnabledAsync(User user, CancellationToken cancellationToken = default)
    {
        var userDataModel = await userManager.FindByIdAsync(user.Id.ToString());

        if (userDataModel is null)
            return false;

        return await userManager.GetTwoFactorEnabledAsync(userDataModel);
    }

    public async Task SignInAsync(User user, CancellationToken cancellationToken = default)
    {
        var userDataModel = await userManager.FindByIdAsync(user.Id.ToString());

        await signInManager.SignInAsync(userDataModel!, true);
    }

    public async Task SignOutAsync(CancellationToken cancellationToken = default)
    {
        await signInManager.SignOutAsync();
    }

    private static string GenerateQrCodeUri(string issuer, string accountName, string sharedKey)
    {
        var encodedIssuer = Uri.EscapeDataString(issuer);
        var encodedAccount = Uri.EscapeDataString(accountName);

        return $"otpauth://totp/{encodedIssuer}:{encodedAccount}?secret={sharedKey}&issuer={encodedIssuer}&digits=6";
    }
}
