using BoyumFoosballStats.Shared.DbModels;
using CosmosDb.Services;

namespace BoyumFoosballStats.Cosmos.Services.Interfaces;

public interface IPlayerCrudService : ICosmosDbCrudService<Player>
{
}