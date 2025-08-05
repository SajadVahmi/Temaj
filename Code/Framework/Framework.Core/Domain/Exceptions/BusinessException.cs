using System.Globalization;
using System.Resources;

namespace Framework.Core.Domain.Exceptions;

public class BusinessException : Exception
{
    public int Code { get; }
    public string? Description { get; }

    public BusinessException(string message) : base(message) { }
    public BusinessException(string message, params object?[] args) : base(string.Format(message, args)) { }
    public BusinessException(int code, string message, string? description = null) : base(message)
    {
        Code = code;
        Description = description;
    }

    public string? GetLocalizedMessage(ResourceManager resourceManager, CultureInfo cultureInfo)
    {
        return resourceManager.GetString(Message, cultureInfo);
    }
}