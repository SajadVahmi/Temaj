using Framework.Core.Application.Commands;
using Idp.Application.UserAggregate.SignInWithSmsOtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Idp.ServiceHost.Pages.VerifyOtpCode;

public class VerifyOtpCodeModel(ICommandBus commandBus) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string PhoneNumber { get; set; } = string.Empty;

    [BindProperty]
    public string OtpCode { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public IActionResult OnGet()
    {
        if (string.IsNullOrWhiteSpace(PhoneNumber))
            return RedirectToPage("/RequestOtpCode/Index", new { returnUrl = ReturnUrl });

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(PhoneNumber))
        {
            ModelState.AddModelError(nameof(PhoneNumber), "Phone number is required.");
            return Page();
        }

        if (string.IsNullOrWhiteSpace(OtpCode))
        {
            ModelState.AddModelError(nameof(OtpCode), "OTP code is required.");
            return Page();
        }

        await commandBus.SendAsync(new SignInWithSmsOtpCommand
        {
            PhoneNumber = PhoneNumber,
            OtpCode = OtpCode
        }, cancellationToken);

        if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            return LocalRedirect(ReturnUrl);

        return RedirectToPage("/Index");
    }
}
