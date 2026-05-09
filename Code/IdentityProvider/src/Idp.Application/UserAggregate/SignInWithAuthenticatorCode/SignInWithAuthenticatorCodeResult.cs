namespace Idp.Application.UserAggregate.SignInWithAuthenticatorCode;

public record SignInWithAuthenticatorCodeResult(bool IsSucceeded, bool IsChallengeExpired);
