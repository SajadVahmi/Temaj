using Framework.Core.Domain.Aggregates;
using Framework.Core.Domain.Exceptions;
using Framework.Core.Domain.Services;
using Framework.Core.Extensions;
using Idp.Domain._Shared.Enums;
using Idp.Domain._Shared.Resources.Exceptions;
using Idp.Domain.UserAggregate.Arguments;
using Idp.Domain.UserAggregate.Events;
using Idp.Domain.UserAggregate.Snapshots;
using Idp.Domain.UserAggregate.ValueObjects;

namespace Idp.Domain.UserAggregate;

public class User : AggregateRoot<long>
{
    public static User FromSnapshot(UserSnapshot snapshot) => new(snapshot);
    public static User Register(RegisterUserArgs args) => new(args.PhoneNumber, args.IdGenerator, args.Clock);

    protected User() { }

    private User(string phoneNumber, IIdGenerator idGenerator, IClock clock)
    {
        if (phoneNumber.IsPhoneNumber())
            PhoneNumber = PhoneNumber.Instantiate(phoneNumber);
        else
            throw new BusinessException(BusinessExceptions.ThePhoneNumberFormatIsNotCorrect);

        Id = idGenerator.GetNewId();


        CheckInvariants();

        AddEvent(new UserRegistered(
            EventId: idGenerator.GetNewId().ToString(),
            UserId: Id,
            Email: Email?.Value,
            IsEmailConfirmed: Email?.IsConfirmed,
            PhoneNumber: PhoneNumber?.Value,
            IsPhoneNumberConfirmed: PhoneNumber?.IsConfirmed,
            TimeOfOccurrence: clock.GetDateTime()));
    }

    private User(UserSnapshot snapshot)
    {
        Id = snapshot.Id;
        Email = snapshot.Email is not null ? Email.Instantiate(snapshot.Email, snapshot.IsEmailConfirmed) : null;
        PhoneNumber = PhoneNumber.Instantiate(snapshot.PhoneNumber, snapshot.IsPhoneNumberConfirmed);
        CheckInvariants();
    }


    public Email? Email { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; } = null!;


    public UserSnapshot GetSnapshot() => new(
            Id: Id,
            Email: Email?.Value,
            IsEmailConfirmed: Email?.IsConfirmed ?? false,
            PhoneNumber: PhoneNumber.Value,
            IsPhoneNumberConfirmed: PhoneNumber?.IsConfirmed ?? false);

    protected sealed override void CheckInvariants()
    {
        if (Email is null && PhoneNumber is null)
            throw new BusinessException(BusinessExceptions.TheUserMustHaveAtLeastOneEmailOrOnePhoneNumber);

    }

    public void RequestSmsOtp(RequestSmsOtpArgs args)
    {
        AddEvent(new SmsOtpRequested(
            EventId: args.IdGenerator.GetNewId().ToString(),
            UserId: Id,
            TimeOfOccurrence: args.Clock.GetDateTime()));
    }

    public async Task SignInWithSmsOtpAsync(SignInWithSmsOtpArgs args, CancellationToken cancellationToken)
    {
        var otpSecrets = await args.OtpSecretRepository.GetByChanelAsync(Id, OtpChanel.Sms, cancellationToken);

        if (otpSecrets is null)
            throw new BusinessException("TheOtpCodeIsNotValid");

        var verified = args.OtpService.ValidateOtp(otpSecrets.SecretKey, args.OtpCode);

        if (!verified)
            throw new BusinessException("TheOtpCodeIsNotValid");

        ConfirmPhoneNumber(new ConfirmPhoneNumberArgs(IdGenerator:args.IdGenerator,Clock:args.Clock));

        await args.SignInService.SignInAsync(this, cancellationToken);

        AddEvent(new UserSignedInWithSmsOtp(
            EventId: args.IdGenerator.GetNewId().ToString(),
            UserId: Id,
            PhoneNumber: PhoneNumber?.Value,
            IsPhoneNumberConfirmed: PhoneNumber?.IsConfirmed,
            TimeOfOccurrence: args.Clock.GetDateTime()));

    }

    public async Task<PasswordSignInStatus> SignInWithPasswordAsync(SignInWithPasswordArgs args, CancellationToken cancellationToken)
    {
        var signInStatus = await args.SignInService.SignInWithPasswordAsync(this, args.Password, cancellationToken);

        if (signInStatus == PasswordSignInStatus.Failed)
            throw new BusinessException("TheUserNameOrPasswordIsNotCorrect");

        if (signInStatus == PasswordSignInStatus.RequiresTwoFactor)
            return PasswordSignInStatus.RequiresTwoFactor;

        AddEvent(new UserSignedInWithPassword(
            EventId: args.IdGenerator.GetNewId().ToString(),
            UserId: Id,
            PhoneNumber: PhoneNumber?.Value,
            IsPhoneNumberConfirmed: PhoneNumber?.IsConfirmed,
            TimeOfOccurrence: args.Clock.GetDateTime()));

        return PasswordSignInStatus.Succeeded;
    }

    public async Task SignOutAsync(SignOutArgs args, CancellationToken cancellationToken)
    {
        await args.SignInService.SignOutAsync(cancellationToken);

        AddEvent(new UserSignedOut(
            EventId:args.IdGenerator.GetNewId().ToString(),
            UserId: Id,
            TimeOfOccurrence: args.Clock.GetDateTime()));
    }


    private void ConfirmPhoneNumber(ConfirmPhoneNumberArgs args)
    {
        if (PhoneNumber is null)
            throw new BusinessException(BusinessExceptions.ForConfirmationTheUserMustHaveAPhoneNumber);

        PhoneNumber = PhoneNumber.Confirm();

        AddEvent(new UserPhoneNumberConfirmed(
            EventId: args.IdGenerator.GetNewId().ToString(),
            UserId: Id,
            PhoneNumber: PhoneNumber.Value,
            IsPhoneNumberConfirmed: PhoneNumber.IsConfirmed,
            TimeOfOccurrence: args.Clock.GetDateTime()));
    }
}
