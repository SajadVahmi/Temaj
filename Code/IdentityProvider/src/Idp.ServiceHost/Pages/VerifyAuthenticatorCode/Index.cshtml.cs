using Framework.Core.Application.Commands;
using Idp.Application.UserAggregate.SignInWithAuthenticatorCode;
using Idp.Domain._Shared.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Idp.ServiceHost.Pages.VerifyAuthenticatorCode;

public class IndexModel(
    ICommandBus commandBus,
    ISignInService signInService) : PageModel
{
    [BindProperty]
    public string VerificationCode { get; set; } = string.Empty;

    [BindProperty]
    public bool RememberMachine { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var hasPendingSignIn = await signInService.HasPendingTwoFactorSignInAsync(cancellationToken);

        if (!hasPendingSignIn)
            return RedirectToPage("/RequestOtpCode/Index", new { returnUrl = ReturnUrl });

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(VerificationCode))
        {
            ModelState.AddModelError(nameof(VerificationCode), "Authenticator code is required.");
            return Page();
        }

        var result = await commandBus.SendAsync<SignInWithAuthenticatorCodeResult>(new SignInWithAuthenticatorCodeCommand
        {
            VerificationCode = VerificationCode,
            RememberMachine = RememberMachine
        }, cancellationToken);

        if (result.IsChallengeExpired)
            return RedirectToPage("/RequestOtpCode/Index", new { returnUrl = ReturnUrl });

        if (!result.IsSucceeded)
        {
            ModelState.AddModelError(nameof(VerificationCode), "Invalid authenticator code.");
            return Page();
        }

        if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            return LocalRedirect(ReturnUrl);

        return RedirectToPage("/Index");
    }
}
