using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Framework.Infrastructure.Persistence.Helpers;

public class SequenceHelper(DbContext dbContext)
{
    public async Task<long> Next(string sequenceName,string schema="dbo")
    {
        var sequenceSqlParameter = new SqlParameter("@sequenceNumber", SqlDbType.BigInt)
        {
            Direction = ParameterDirection.Output
        };

        await dbContext.Database.ExecuteSqlRawAsync($"set @sequenceNumber = NEXT VALUE FOR {schema}.{sequenceName}", sequenceSqlParameter);

        return (long)sequenceSqlParameter.Value;
    }
}