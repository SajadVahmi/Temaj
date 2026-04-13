using System.Data;
using Dapper;
using Framework.Infrastructure.Persistence.Events;

namespace Framework.Infrastructure.EventProcessor.EventSubscriber.DataStore.SqlServer;

public static class SqlServerQueries
{
    public static long GetCursorPosition(this IDbConnection connection, SqlServerEventStoreSubscriberConfig subscriberConfig,string subscriptionName)
    {
        return connection.Query<long>($"SELECT Position FROM {subscriberConfig.CursorTable} WHERE Id='{subscriptionName}'").First();
    }
    public static void MovePosition(this IDbConnection connection, SqlServerEventStoreSubscriberConfig subscriberConfig, long position, string subscriptionName)
    {
        connection.Execute($"UPDATE {subscriberConfig.CursorTable} SET Position={position} WHERE Id='{subscriptionName}'");
    }

    public static List<EventItem> GetEventsFromPosition(this IDbConnection connection, SqlServerEventStoreSubscriberConfig subscriberConfig,long position)
    {
        var query = $"SELECT * FROM {subscriberConfig.EventTable} WHERE Id > {position} ORDER BY Id";
        return connection.Query<EventItem>(query).ToList();
    }

    public static void CreateCursorTableIfNotExist(this IDbConnection connection, SqlServerEventStoreSubscriberConfig subscriberConfig, string subscriptionName)
    {
        var parameters = new DynamicParameters();
        parameters.Add("Id", subscriptionName);
        parameters.Add("Position", 0);

        string creationScript = $"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{subscriberConfig.CursorTable}') BEGIN  CREATE TABLE [dbo].[{subscriberConfig.CursorTable}]([Id] [varchar](100) NOT NULL, [Position] [bigint] NOT NULL,  CONSTRAINT [PK_Cursor] PRIMARY KEY CLUSTERED  ([Id] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF ) ON [PRIMARY]) ON [PRIMARY] ALTER TABLE [dbo].[{subscriberConfig.CursorTable}] ADD  CONSTRAINT [DF_{subscriberConfig.CursorTable}_Position]  DEFAULT ((0)) FOR Position END";

        connection.Execute(creationScript, parameters);

        var existRowCount = connection.ExecuteScalar<long>($"SELECT COUNT([Id]) FROM [dbo].[{subscriberConfig.CursorTable}] WHERE [Id] = @Id", parameters);


        if (existRowCount == 0)
            connection.Execute($"INSERT INTO [dbo].[{subscriberConfig.CursorTable}] ([Id] ,[Position]) VALUES (@Id,@Position)", parameters);

    }

}