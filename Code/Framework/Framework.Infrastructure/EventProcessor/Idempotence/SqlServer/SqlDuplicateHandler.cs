using Framework.Core.Domain.Services;
using Microsoft.Data.SqlClient;

namespace Framework.Infrastructure.EventProcessor.Idempotence.SqlServer;

public class SqlDuplicateHandler(SqlDuplicateHandlerConfig config,IClock clock) : IDuplicateEventHandler
{
    public async Task<bool> HasMessageBeenProcessed(string consumerName, string eventId)
    {
        await using var connection = new SqlConnection(config.ConnectionString);
        return await connection.CheckEventExistAsync(config.TableName!, eventId,consumerName);
    }

    public async Task MarkMessageAsProcessed( string messageId, string consumerName, string typeName,string payload ,DateTimeOffset receivedDateTime)
    {
        await using var connection = new SqlConnection(config.ConnectionString);
        await connection.MarkAsReceivedAsync(
            tableName:config.TableName!,
            messageId:messageId,
            consumerName: consumerName,
            typeName:typeName,
            payload:payload,
            clock:clock);
    }
    
}