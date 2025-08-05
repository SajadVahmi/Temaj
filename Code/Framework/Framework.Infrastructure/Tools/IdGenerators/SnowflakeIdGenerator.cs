using Framework.Core.Domain.Services;
using IdGen;

namespace Framework.Infrastructure.Tools.IdGenerators;

public class SnowflakeIdGenerator(IdGenerator idGenerator) : IIdGenerator
{
    public long GetNewId() => idGenerator.CreateId();
}