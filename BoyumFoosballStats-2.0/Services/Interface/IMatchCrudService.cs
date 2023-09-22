using BoyumFoosballStats_2._0.Shared.DbModels;
using CosmosDb.Services;

namespace BoyumFoosballStats_2._0.Services.Interface;

public interface IMatchCrudService : ICosmosDbCrudService<Match>
{
}