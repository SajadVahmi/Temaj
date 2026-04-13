using System.Text.RegularExpressions;

namespace Framework.Core.Extensions;

public static class StringExtensions
{
    public const string Email = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

    public const string PhoneNumber = @"^\+[1-9]\d{1,14}$";

    public static bool IsPhoneNumber(this string input) => Regex.IsMatch(input, PhoneNumber);
    public static bool IsEmail(this string input) => Regex.IsMatch(input, Email);
    public static string ToCamelCase(this string text)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            text = text.TrimStart();
            return char.ToLower(text[0]) + text.Substring(1);
        }

        return text;
    }

}