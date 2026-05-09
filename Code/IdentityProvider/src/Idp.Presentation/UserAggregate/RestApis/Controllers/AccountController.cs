using Framework.Core.Application.Commands;
using Framework.Core.Domain.Services;
using Idp.Application.UserAggregate.ManageAuthenticatorTwoFactor;
using Idp.Application.UserAggregate.RequestSmsOtp;
using Idp.Application.UserAggregate.SignInWithAuthenticatorCode;
using Idp.Application.UserAggregate.SignInWithPassword;
using Idp.Application.UserAggregate.SignInWithSmsOtp;
using Idp.Application.UserAggregate.SignOut;
using Idp.Presentation.UserAggregate.RestApis.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idp.Presentation.UserAggregate.RestApis.Controllers;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{

    [HttpPost("request-otp")]
    public async Task<IActionResult> RequestOtpAsync(
        [FromBody] RequestSmsOtpBody request,
        [FromServices] ICommandBus commandBus,
        //[FromServices] IValidatorAdapter validator,
        CancellationToken cancellationToken = default)
    {
        // var validationResult = validator.Validate(request);

        var command = new RequestSmsOtpCommand { PhoneNumber = request.PhoneNumber! };

        await commandBus.SendAsync(command, cancellationToken);

        return Created();

    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignInWithSmsOtpAsync(
        [FromBody] VerifySmsOtpBody request,
        [FromServices] ICommandBus commandBus,
        //[FromServices] IValidatorAdapter validator,
        CancellationToken cancellationToken = default)
    {
        // var validationResult = validator.Validate(request);

        var command = new SignInWithSmsOtpCommand() { PhoneNumber = request.PhoneNumber!, OtpCode = request.OtpCode! };

        await commandBus.SendAsync(command, cancellationToken);

        return Created();

    }

    [HttpPost("sign-in-with-password")]
    public async Task<IActionResult> SignInWithPasswordAsync(
        [FromBody] SignInWithPasswordBody request,
        [FromServices] ICommandBus commandBus,
        CancellationToken cancellationToken = default)
    {
        var phoneNumberOrEmail = request.PhoneNumberOrEmail ?? request.PhoneNumber;

        var command = new SignInWithPasswordCommand()
        {
            PhoneNumberOrEmail = phoneNumberOrEmail!,
            Password = request.Password!
        };

        var result = await commandBus.SendAsync<SignInWithPasswordResult>(command, cancellationToken);

        if (result.RequiresTwoFactor)
            return Accepted(new { requiresTwoFactor = true });

        return Created();
    }

    [HttpPost("sign-in-with-authenticator")]
    public async Task<IActionResult> SignInWithAuthenticatorAsync(
        [FromBody] SignInWithAuthenticatorCodeBody request,
        [FromServices] ICommandBus commandBus,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.VerificationCode))
            return BadRequest(new { error = "Verification code is required." });

        var result = await commandBus.SendAsync<SignInWithAuthenticatorCodeResult>(new SignInWithAuthenticatorCodeCommand
        {
            VerificationCode = request.VerificationCode!,
            RememberMachine = request.RememberMachine
        }, cancellationToken);

        if (result.IsChallengeExpired)
            return BadRequest(new { error = "Two-factor authentication challenge expired. Please sign in again." });

        if (!result.IsSucceeded)
            return BadRequest(new { error = "Invalid authenticator code." });

        return Created();
    }

    [HttpPost("sign-out")]
    public async Task<IActionResult> SignOutAsync(
        [FromServices] ICommandBus commandBus,
        CancellationToken cancellationToken = default)
    {
        // var validationResult = validator.Validate(request);

        var command = new SignOutCommand();

        await commandBus.SendAsync(command, cancellationToken);

        return Created();

    }

    [Authorize(AuthenticationSchemes = "Identity.Application")]
    [HttpGet("two-factor-authenticator/setup")]
    public async Task<IActionResult> GetAuthenticatorSetupAsync(
        [FromServices] ICommandBus commandBus,
        [FromServices] IIdentityService identityService,
        CancellationToken cancellationToken = default)
    {
        var result = await commandBus.SendAsync<GetAuthenticatorSetupResult>(new GetAuthenticatorSetupCommand
        {
            UserId = identityService.RequiredCurrentUserId,
            Issuer = Request.Host.Value
        }, cancellationToken);

        return Ok(result);
    }

    [Authorize(AuthenticationSchemes = "Identity.Application")]
    [HttpPost("two-factor-authenticator/enable")]
    public async Task<IActionResult> EnableAuthenticatorTwoFactorAsync(
        [FromBody] EnableAuthenticatorTwoFactorBody request,
        [FromServices] ICommandBus commandBus,
        [FromServices] IIdentityService identityService,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.VerificationCode))
            return BadRequest(new { error = "Verification code is required." });

        var result = await commandBus.SendAsync<EnableAuthenticatorTwoFactorResult>(new EnableAuthenticatorTwoFactorCommand
        {
            UserId = identityService.RequiredCurrentUserId,
            VerificationCode = request.VerificationCode!
        }, cancellationToken);

        if (!result.IsSucceeded)
            return BadRequest(new { error = "Invalid authenticator code." });

        return Created();
    }

    [Authorize(AuthenticationSchemes = "Identity.Application")]
    [HttpPost("two-factor-authenticator/disable")]
    public async Task<IActionResult> DisableAuthenticatorTwoFactorAsync(
        [FromServices] ICommandBus commandBus,
        [FromServices] IIdentityService identityService,
        CancellationToken cancellationToken = default)
    {
        await commandBus.SendAsync(new DisableAuthenticatorTwoFactorCommand
        {
            UserId = identityService.RequiredCurrentUserId
        }, cancellationToken);

        return Created();
    }
}
