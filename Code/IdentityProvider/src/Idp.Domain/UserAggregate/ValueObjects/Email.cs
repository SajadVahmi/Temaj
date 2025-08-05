using Framework.Core.Domain.ValueObjects;
using Idp.Domain._Shared.Constants;
using System.Text.RegularExpressions;
using Framework.Core.Domain.Exceptions;
using Idp.Domain._Shared.Resources.Exceptions;

namespace Idp.Domain.UserAggregate.ValueObjects;

public class Email : ValueObject<Email>
{
    public static Email Instantiate(string value) => new(value,false);
    public static Email Instantiate(string value, bool isConfirmed) => new(value,isConfirmed);
    

    protected Email()
    {
    }

    protected Email(string value,bool isConfirmed)
    {
        Value = value;
        IsConfirmed = isConfirmed;
        Validate();
    }

    public string Value { get; } = default!;
    public bool IsConfirmed { get; }

    public Email Confirm()
    {
        return new Email(Value, true);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public Email Clone()
    {
        return Instantiate(Value,IsConfirmed);
    }
    public sealed override void Validate()
    {
        if (!Regex.IsMatch(Value, RegularExpressions.Email))
            throw new BusinessException(BusinessExceptions.TheEmailFormatIsNotCorrect);
    }
}