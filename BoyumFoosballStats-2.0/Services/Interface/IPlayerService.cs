using BoyumFoosballStats_2._0.Shared.FirestoreModels;
using Firestore.Services;

namespace BoyumFoosballStats_2._0.Services.Interface;

public interface IPlayerService : IFirestoreCrudController<Player>
{
}