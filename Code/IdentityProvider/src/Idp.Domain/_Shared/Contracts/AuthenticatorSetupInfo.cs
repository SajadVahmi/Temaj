namespace Idp.Domain._Shared.Contracts;

public record AuthenticatorSetupInfo(
    string SharedKey,
    string QrCodeUri,
    bool IsTwoFactorEnabled,
    string AccountName
);
