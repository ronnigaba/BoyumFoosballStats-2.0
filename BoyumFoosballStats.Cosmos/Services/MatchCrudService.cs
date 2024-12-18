﻿using BoyumFoosballStats.Cosmos.Services.Interfaces;
using BoyumFoosballStats.Shared;
using BoyumFoosballStats.Shared.DbModels;
using CosmosDb.Model;
using CosmosDb.Services;
using Microsoft.Extensions.Options;

namespace BoyumFoosballStats.Cosmos.Services;

public class MatchCrudService : CosmosDbCrudService<Match>, IMatchCrudService
{
    public MatchCrudService(IOptions<CosmosDbSettings> dbSettings) : base(dbSettings,
        BoyumFoosballStatsConsts.MatchesContainerName, BoyumFoosballStatsConsts.MatchesPartitionKey)
    {
    }
}

public class DebugMatchCrudService : CosmosDbCrudService<Match>, IMatchCrudService
{
    public DebugMatchCrudService(IOptions<CosmosDbSettings> dbSettings) : base(dbSettings,
        BoyumFoosballStatsConsts.DebugPrefix + BoyumFoosballStatsConsts.MatchesContainerName,
        BoyumFoosballStatsConsts.MatchesPartitionKey)
    {
    }
}