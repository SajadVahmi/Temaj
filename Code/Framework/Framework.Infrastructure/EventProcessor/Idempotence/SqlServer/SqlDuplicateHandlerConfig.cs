namespace Framework.Infrastructure.EventProcessor.Idempotence.SqlServer
{
    public class SqlDuplicateHandlerConfig
    {
        public string? TableName { get; set; }
        public string? ConnectionString { get; set; }
    }
}
