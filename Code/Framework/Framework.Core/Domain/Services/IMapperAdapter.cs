namespace Framework.Core.Domain.Services;

public interface IMapperAdapter
{
    public TDestination Map<TDestination>(object source);
}