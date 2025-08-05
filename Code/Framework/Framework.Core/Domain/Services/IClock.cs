namespace Framework.Core.Domain.Services;

public interface IClock
{
    public DateTimeOffset GetDateTime();
    public DateOnly GetDate();

}