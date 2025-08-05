using Framework.Core.Domain.Aggregates;
using Framework.Core.Domain.Exceptions;
using Framework.Core.Domain.Services;
using Framework.Core.Extensions;
using Idp.Domain._Shared.Resources.Exceptions;
using Idp.Domain.UserAggregate.Arguments;
using Idp.Domain.UserAggregate.Events;
using Idp.Domain.UserAggregate.Snapshots;
using Idp.Domain.UserAggregate.ValueObjects;

namespace Idp.Domain.UserAggregate;

public class User : AggregateRoot<long>
{
    public static User FromSnapshot(UserSnapshot snapshot) => new(snapshot);
    public static User Register(RegisterUserArgs args) => new(args.UserName, args.IdGenerator, args.Clock);
    

    protected User() { }

    private User(string userName, IIdGenerator idGenerator, IClock clock)
    {
        if(userName.IsPhoneNumber())
            PhoneNumber =  PhoneNumber.Instantiate(userName);

        else if (userName.IsEmail())
            Email= Email.Instantiate(userName);

        else
            throw new BusinessException(BusinessExceptions.EmailOrPhoneNumberFormatIsNotCorrect);

        Id = idGenerator.GetNewId();
        
       
        CheckInvariants();

        AddEvent(new UserRegistered(
            EventId:idGenerator.GetNewId().ToString(),
            UserId:Id,
            Email:Email?.Value,
            IsEmailConfirmed: Email?.IsConfirmed,
            PhoneNumber: PhoneNumber?.Value,
            IsPhoneNumberConfirmed: PhoneNumber?.IsConfirmed,
            TimeOfOccurrence:clock.GetDateTime()));
    }

    private User(UserSnapshot snapshot)
    {
        Id = snapshot.Id;
        Email = snapshot.Email is not null ? Email.Instantiate(snapshot.Email, snapshot.IsEmailConfirmed) : null;
        PhoneNumber = snapshot.PhoneNumber is not null ? PhoneNumber.Instantiate(snapshot.PhoneNumber, snapshot.IsPhoneNumberConfirmed) : null;
        CheckInvariants();
    }


    public Email? Email { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }


    public void ConfirmPhoneNumber(ConfirmPhoneNumberArgs args)
    {
        if (PhoneNumber is null)
            throw new BusinessException(BusinessExceptions.ForConfirmationTheUserMustHaveAPhoneNumber);

        PhoneNumber = PhoneNumber.Confirm();

        AddEvent(new UserPhoneNumberConfirmed(
            EventId: args.IdGenerator.GetNewId().ToString(),
            UserId: Id,
            TimeOfOccurrence: args.Clock.GetDateTime()));
    }

    public void ConfirmEmail(ConfirmEmailArgs args)
    {
        if (Email is null)
            throw new BusinessException(BusinessExceptions.ForConfirmationTheUserMustHaveAnEmail);

        Email = Email.Confirm();

        AddEvent(new UserEmailConfirmed(
            EventId: args.IdGenerator.GetNewId().ToString(),
            UserId: Id,
            TimeOfOccurrence: args.Clock.GetDateTime()));
    }


    public UserSnapshot GetSnapshot() => new(
            Id: Id,
            Email: Email?.Value,
            IsEmailConfirmed: Email?.IsConfirmed ?? false,
            PhoneNumber: PhoneNumber?.Value,
            IsPhoneNumberConfirmed: PhoneNumber?.IsConfirmed ?? false);


    protected sealed override void CheckInvariants()
    {
        if (Email is null && PhoneNumber is null)
            throw new BusinessException(BusinessExceptions.TheUserMustHaveAtLeastOneEmailOrOnePhoneNumber);

    }
}