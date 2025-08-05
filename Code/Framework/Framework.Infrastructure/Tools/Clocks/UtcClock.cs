using Framework.Core.Domain.Services;

namespace Framework.Infrastructure.Tools.Clocks;

public class UtcClock : IClock
{
    public DateOnly GetDate()
    {
        return DateOnly.FromDateTime(DateTimeOffset.UtcNow.DateTime);
    }

    public DateTimeOffset GetDateTime()
    {
        return DateTimeOffset.UtcNow;
    }
}