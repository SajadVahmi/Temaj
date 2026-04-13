namespace Framework.Infrastructure.Queries
{
    public interface IQueryBus
    {
        public Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken=default);
    }


}
