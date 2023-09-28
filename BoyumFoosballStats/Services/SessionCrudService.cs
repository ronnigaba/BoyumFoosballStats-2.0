using BoyumFoosballStats.Services.Interface;
using BoyumFoosballStats.Shared;
using BoyumFoosballStats.Shared.DbModels;
using CosmosDb.Model;
using CosmosDb.Services;
using Microsoft.Extensions.Options;

namespace BoyumFoosballStats.Services;

public class SessionCrudService : CosmosDbCrudService<Session>, ISessionCrudService
{
    public SessionCrudService(IOptions<CosmosDbSettings> dbSettings) : base(dbSettings,
        BoyumFoosballStatsConsts.SessionsContainerName, BoyumFoosballStatsConsts.SessionsPartitionKey)
    {
    }
}