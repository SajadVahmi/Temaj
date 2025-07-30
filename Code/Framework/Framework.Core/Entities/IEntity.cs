namespace Framework.Core.Entities;

public interface IEntity<out TId>
{
    public TId Id { get;}
}
