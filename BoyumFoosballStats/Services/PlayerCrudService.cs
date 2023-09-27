using BoyumFoosballStats.Services.Interface;
using BoyumFoosballStats.Shared;
using BoyumFoosballStats.Shared.DbModels;
using CosmosDb.Model;
using CosmosDb.Services;
using Microsoft.Extensions.Options;

namespace BoyumFoosballStats.Services;

public class PlayerCrudService : CosmosDbCrudService<Player>, IPlayerCrudService
{
    public PlayerCrudService(IOptions<CosmosDbSettings> dbSettings) : base(dbSettings,
        BoyumFoosballStatsConsts.PlayerContainernName, BoyumFoosballStatsConsts.PlayerPartitionKey)
    {
    }
}