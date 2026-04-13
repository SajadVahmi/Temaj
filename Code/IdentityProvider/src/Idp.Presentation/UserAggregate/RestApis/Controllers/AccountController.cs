using Framework.Core.Application.Commands;
using Idp.Application.UserAggregate.RequestSmsOtp;
using Idp.Application.UserAggregate.SignInWithPassword;
using Idp.Application.UserAggregate.SignInWithSmsOtp;
using Idp.Application.UserAggregate.SignOut;
using Idp.Presentation.UserAggregate.RestApis.Requests;
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
        var command = new SignInWithPasswordCommand()
        {
            PhoneNumber = request.PhoneNumber!,
            Password = request.Password!
        };

        await commandBus.SendAsync(command, cancellationToken);

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
}