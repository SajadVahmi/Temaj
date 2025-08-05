namespace Idp.Domain.UserAggregate.Snapshots;

public record UserSnapshot(
    long Id,
    string? Email,
    bool IsEmailConfirmed,
    string? PhoneNumber,
    bool IsPhoneNumberConfirmed
);
