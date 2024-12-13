using BoyumFoosballStats.Cosmos.Services.Interfaces;
using BoyumFoosballStats.Shared;
using BoyumFoosballStats.Shared.DbModels;
using CosmosDb.Model;
using CosmosDb.Services;
using Microsoft.Extensions.Options;

namespace BoyumFoosballStats.Cosmos.Services;

public class PlayerCrudService : CosmosDbCrudService<Player>, IPlayerCrudService
{
    public PlayerCrudService(IOptions<CosmosDbSettings> dbSettings) : base(dbSettings,
        BoyumFoosballStatsConsts.PlayerContainerName, BoyumFoosballStatsConsts.PlayerPartitionKey)
    {
    }
}

public class DebugPlayerCrudService : CosmosDbCrudService<Player>, IPlayerCrudService
{
    public DebugPlayerCrudService(IOptions<CosmosDbSettings> dbSettings) : base(dbSettings,
        BoyumFoosballStatsConsts.DebugPrefix + BoyumFoosballStatsConsts.PlayerContainerName,
        BoyumFoosballStatsConsts.PlayerPartitionKey)
    {
    }
}