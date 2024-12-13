using BoyumFoosballStats.Cosmos.Services.Interfaces;
using BoyumFoosballStats.Shared;
using BoyumFoosballStats.Shared.DbModels;
using CosmosDb.Model;
using CosmosDb.Services;
using Microsoft.Extensions.Options;

namespace BoyumFoosballStats.Cosmos.Services;

public class SessionCrudService : CosmosDbCrudService<Session>, ISessionCrudService
{
    public SessionCrudService(IOptions<CosmosDbSettings> dbSettings) : base(dbSettings,
        BoyumFoosballStatsConsts.SessionsContainerName, BoyumFoosballStatsConsts.SessionsPartitionKey)
    {
    }
}

public class DebugSessionCrudService : CosmosDbCrudService<Session>, ISessionCrudService
{
    public DebugSessionCrudService(IOptions<CosmosDbSettings> dbSettings) : base(dbSettings,
        BoyumFoosballStatsConsts.DebugPrefix + BoyumFoosballStatsConsts.SessionsContainerName,
        BoyumFoosballStatsConsts.SessionsPartitionKey)
    {
    }
}