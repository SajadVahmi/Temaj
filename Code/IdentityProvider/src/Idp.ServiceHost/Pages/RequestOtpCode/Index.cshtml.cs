using Framework.Core.Application.Commands;
using Idp.Application.UserAggregate.RequestSmsOtp;
using Idp.Application.UserAggregate.SignInWithPassword;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Idp.ServiceHost.Pages.RequestOtpCode;

public class IndexModel(ICommandBus commandBus) : PageModel
{
    [BindProperty]
    public string OtpPhoneNumber { get; set; } = string.Empty;

    [BindProperty]
    public string PasswordPhoneNumber { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public IActionResult OnGet() => Page();

    public async Task<IActionResult> OnPostRequestOtpAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(OtpPhoneNumber))
        {
            ModelState.AddModelError(nameof(OtpPhoneNumber), "Phone number is required.");
            return Page();
        }

        await commandBus.SendAsync(new RequestSmsOtpCommand { PhoneNumber = OtpPhoneNumber }, cancellationToken);

        return RedirectToPage("/VerifyOtpCode/Index", new { phoneNumber = OtpPhoneNumber, returnUrl = ReturnUrl });
    }

    public async Task<IActionResult> OnPostPasswordAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(PasswordPhoneNumber))
            ModelState.AddModelError(nameof(PasswordPhoneNumber), "Phone number is required.");

        if (string.IsNullOrWhiteSpace(Password))
            ModelState.AddModelError(nameof(Password), "Password is required.");

        if (!ModelState.IsValid)
            return Page();

        await commandBus.SendAsync(new SignInWithPasswordCommand
        {
            PhoneNumber = PasswordPhoneNumber,
            Password = Password
        }, cancellationToken);

        return RedirectToReturnUrlOrHome();
    }

    private IActionResult RedirectToReturnUrlOrHome()
    {
        if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            return LocalRedirect(ReturnUrl);

        return RedirectToPage("/Index");
    }
}
