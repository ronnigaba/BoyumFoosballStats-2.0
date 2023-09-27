using BoyumFoosballStats_2._0.Services.Interface;
using BoyumFoosballStats_2._0.Shared;
using BoyumFoosballStats_2._0.Shared.DbModels;
using CosmosDb.Model;
using CosmosDb.Services;
using Microsoft.Extensions.Options;

namespace BoyumFoosballStats_2._0.Services;

public class MatchCrudService : CosmosDbCrudService<Match>, IMatchCrudService
{
    public MatchCrudService(IOptions<CosmosDbSettings> dbSettings) : base(dbSettings,
        BoyumFoosballStatsConsts.MatchesContainerName, BoyumFoosballStatsConsts.PlayerPartitionKey)
    {
    }
}