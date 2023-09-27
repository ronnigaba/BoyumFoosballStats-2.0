using BoyumFoosballStats.Services.Interface;
using BoyumFoosballStats.Shared;
using BoyumFoosballStats.Shared.DbModels;
using CosmosDb.Model;
using CosmosDb.Services;
using Microsoft.Extensions.Options;

namespace BoyumFoosballStats.Services;

public class MatchCrudService : CosmosDbCrudService<Match>, IMatchCrudService
{
    public MatchCrudService(IOptions<CosmosDbSettings> dbSettings) : base(dbSettings,
        BoyumFoosballStatsConsts.MatchesContainerName, BoyumFoosballStatsConsts.MatchesPartitionKey)
    {
    }
}