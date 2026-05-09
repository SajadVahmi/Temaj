using System.Security.Claims;
using Framework.Core.Application.Commands;
using Idp.Application.UserAggregate.ManageAuthenticatorTwoFactor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;

namespace Idp.ServiceHost.Pages.Profile;

[Authorize(AuthenticationSchemes = "Identity.Application")]
public class SettingsModel(ICommandBus commandBus) : PageModel
{
    [BindProperty]
    public string VerificationCode { get; set; } = string.Empty;

    public bool IsTwoFactorEnabled { get; private set; }
    public string SharedKey { get; private set; } = string.Empty;
    public string AccountName { get; private set; } = string.Empty;
    public string QrCodeImageDataUri { get; private set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
            return RedirectToPage("/RequestOtpCode/Index");

        await LoadSetupAsync(userId.Value, cancellationToken);

        return Page();
    }

    public async Task<IActionResult> OnPostEnableAsync(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
            return RedirectToPage("/RequestOtpCode/Index");

        if (string.IsNullOrWhiteSpace(VerificationCode))
            ModelState.AddModelError(nameof(VerificationCode), "Authenticator code is required.");

        if (!ModelState.IsValid)
        {
            await LoadSetupAsync(userId.Value, cancellationToken);
            return Page();
        }

        var result = await commandBus.SendAsync<EnableAuthenticatorTwoFactorResult>(new EnableAuthenticatorTwoFactorCommand
        {
            UserId = userId.Value,
            VerificationCode = VerificationCode
        }, cancellationToken);

        if (!result.IsSucceeded)
            ModelState.AddModelError(nameof(VerificationCode), "Invalid authenticator code.");
        else
            VerificationCode = string.Empty;

        await LoadSetupAsync(userId.Value, cancellationToken);

        return Page();
    }

    public async Task<IActionResult> OnPostDisableAsync(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
            return RedirectToPage("/RequestOtpCode/Index");

        await commandBus.SendAsync(new DisableAuthenticatorTwoFactorCommand
        {
            UserId = userId.Value
        }, cancellationToken);

        await LoadSetupAsync(userId.Value, cancellationToken);

        return Page();
    }

    private async Task LoadSetupAsync(long userId, CancellationToken cancellationToken)
    {
        var setup = await commandBus.SendAsync<GetAuthenticatorSetupResult>(new GetAuthenticatorSetupCommand
        {
            UserId = userId,
            Issuer = Request.Host.Value
        }, cancellationToken);

        IsTwoFactorEnabled = setup.IsTwoFactorEnabled;
        AccountName = setup.AccountName;
        SharedKey = FormatKey(setup.SharedKey);
        QrCodeImageDataUri = GenerateQrCodeDataUri(setup.QrCodeUri);
    }

    private long? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (long.TryParse(userIdClaim, out var userId))
            return userId;

        return null;
    }

    private static string FormatKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return string.Empty;

        return string.Join(" ", Enumerable.Range(0, key.Length / 4 + (key.Length % 4 == 0 ? 0 : 1))
            .Select(i => key.Substring(i * 4, Math.Min(4, key.Length - i * 4))));
    }

    private static string GenerateQrCodeDataUri(string content)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrCodeData);
        var bytes = qrCode.GetGraphic(20);
        return $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
    }
}
