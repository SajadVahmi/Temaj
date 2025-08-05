namespace Idp.Domain._Shared.Constants;

public static class RegularExpressions
{
    public const string Email =@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

    public const string PhoneNumber= @"^\+[1-9]\d{1,14}$";
}
