using BoyumFoosballStats_2._0.Services.Interface;
using BoyumFoosballStats_2._0.Shared;
using BoyumFoosballStats_2._0.Shared.FirestoreModels;
using Firestore.Controllers;

namespace BoyumFoosballStats_2._0.Services;

public class PlayerService : FirestoreCrudController<Player>, IPlayerService
{
    public PlayerService() : base(BoyumFoosballStatsConsts.ProjectName, BoyumFoosballStatsConsts.PlayerCollectionName)
    {
    }
}