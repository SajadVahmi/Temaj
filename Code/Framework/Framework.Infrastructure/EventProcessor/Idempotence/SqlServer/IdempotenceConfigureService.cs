using Framework.Core.Domain.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;

namespace Framework.Infrastructure.EventProcessor.Idempotence.SqlServer;

public static class IdempotenceConfigureService
{
    public static IServiceCollection UseInboxPatternWithSqlServer(this IServiceCollection services, Action<SqlDuplicateHandlerConfig> config)
    {
        var sqlConfig = new SqlDuplicateHandlerConfig();
       


        config.Invoke(sqlConfig);
        ArgumentNullException.ThrowIfNull(sqlConfig);
        ArgumentNullException.ThrowIfNull(sqlConfig.TableName);
        ArgumentNullException.ThrowIfNull(sqlConfig.ConnectionString);


        services.AddSingleton(sqlConfig);

        using var connection = new SqlConnection(sqlConfig.ConnectionString);
        connection.CreateInboxTableIfNotExist(
           tableName: sqlConfig.TableName!);

        services.AddSingleton<IDuplicateEventHandler, SqlDuplicateHandler>();

        var clockService = services.BuildServiceProvider().GetService<IClock>();

        ArgumentNullException.ThrowIfNull(clockService);

        return services;
    }
}