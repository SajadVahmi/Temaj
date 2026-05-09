namespace Idp.Application.UserAggregate.ManageAuthenticatorTwoFactor;

public record GetAuthenticatorSetupResult(
    string SharedKey,
    string QrCodeUri,
    string AccountName,
    bool IsTwoFactorEnabled
);
