using Framework.Core.Domain.Exceptions;
using Framework.Core.Domain.ValueObjects;
using Idp.Domain._Shared.Constants;
using Idp.Domain._Shared.Resources.Exceptions;
using System.Text.RegularExpressions;

namespace Idp.Domain.UserAggregate.ValueObjects;

public class PhoneNumber : ValueObject<PhoneNumber>
{
    public static PhoneNumber Instantiate(string value) => new(value, false);
    public static PhoneNumber Instantiate(string value, bool isConfirmed) => new(value, isConfirmed);


    protected PhoneNumber()
    {
    }

    protected PhoneNumber(string value, bool isConfirmed)
    {
        Value = value;
        IsConfirmed = isConfirmed;
        Validate();
    }

    public string Value { get; } = default!;

    public bool IsConfirmed { get; }

    public PhoneNumber Confirm()
    {
        return new PhoneNumber(Value, true);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
        yield return IsConfirmed;
    }

    public PhoneNumber Clone()
    {
        return Instantiate(Value,IsConfirmed);
    }
    public sealed override void Validate()
    {
        if (!Regex.IsMatch(Value, RegularExpressions.PhoneNumber))
            throw new BusinessException(BusinessExceptions.ThePhoneNumberFormatIsNotCorrect);
    }
}