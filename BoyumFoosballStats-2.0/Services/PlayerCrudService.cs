using BoyumFoosballStats_2._0.Services.Interface;
using BoyumFoosballStats_2._0.Shared;
using BoyumFoosballStats_2._0.Shared.FirestoreModels;
using Firestore.Services;

namespace BoyumFoosballStats_2._0.Services;

public class PlayerCrudService : FirestoreCrudService<Player>, IPlayerService
{
    public PlayerCrudService() : base(BoyumFoosballStatsConsts.ProjectName, BoyumFoosballStatsConsts.PlayerCollectionName)
    {
    }
}