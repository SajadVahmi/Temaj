using System.Data;
using Dapper;
using Framework.Core.Domain.Services;

namespace Framework.Infrastructure.EventProcessor.Idempotence.SqlServer;

public static class SqlQueries
{
    public static async Task<bool> CheckEventExistAsync(this IDbConnection connection, string tableName, string messageId, string consumerName, CancellationToken cancellationToken = default)
    {
        string query = $"SELECT COUNT(MessageId) FROM {tableName} WHERE MessageId=@MessageId AND ConsumerName=@ConsumerName";
        var result = await connection.QueryFirstOrDefaultAsync<long>(query, new
        {
            MessageId = messageId,
            ConsumerName = consumerName
        });
        return result > 0;
    }

    public static async Task MarkAsReceivedAsync(this IDbConnection connection,  string tableName, string consumerName, string messageId, string typeName, string payload, IClock clock, CancellationToken cancellationToken = default)
    {
        string query = $"INSERT INTO {tableName} ([MessageId],[ConsumerName],[TypeName],[Payload],[ReceivedDateTime]) VALUES(@MessageId,@ConsumerName,@TypeName,@Payload,@ReceivedDate)";
        await connection.ExecuteAsync(query, new {MessageId = messageId, ConsumerName = consumerName, TypeName =typeName, Payload=payload, ReceivedDate = clock.GetDateTime() });
    }

    public static void CreateInboxTableIfNotExist(this IDbConnection connection, string tableName)
    {
        connection.Execute($@"
                                   IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = '{tableName}')
                                   BEGIN
                                       CREATE TABLE [{tableName}] (
                                           [Id] BIGINT IDENTITY(1,1),
                                           [MessageId] NVARCHAR(100) NOT NULL,
                                           [ConsumerName] NVARCHAR(100) NOT NULL,
                                           [TypeName] NVARCHAR(100) NOT NULL,
                                           [Payload] NVARCHAR(Max) NOT NULL,
                                           [ReceivedDateTime] DATETIMEOFFSET NOT NULL,
                                           CONSTRAINT [PK_{tableName}] PRIMARY KEY ([Id])
                                       );
                                   END;");
    }
}