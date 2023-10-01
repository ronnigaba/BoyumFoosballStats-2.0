using BoyumFoosballStats.Shared.DbModels;
using CosmosDb.Services;

namespace BoyumFoosballStats.Services.Interface;

public interface ISessionCrudService : ICosmosDbCrudService<Session>
{
}