using FluentValidation;
using Framework.Core.Extensions;
using Idp.Presentation._Shared.Resources;
using Idp.Presentation.UserAggregate.RestApis.Requests;

namespace Idp.Presentation.UserAggregate.RestApis.Validators;

public class RequestSmsOtpBodyValidator : AbstractValidator<RequestSmsOtpBody>
{
    public RequestSmsOtpBodyValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ApiValidationMessages.InvalidPhoneNumberFormat)
            .NotEmpty().WithMessage(ApiValidationMessages.PhoneNumberCannotBeEmpty)
            .Must(p => !string.IsNullOrWhiteSpace(p)).WithMessage(ApiValidationMessages.PhoneNumberCannotBeEmpty)
            .Must(p=>!p?.IsPhoneNumber()==true).WithMessage(ApiValidationMessages.InvalidPhoneNumberFormat);
    }
}