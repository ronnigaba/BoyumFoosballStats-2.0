using BoyumFoosballStats_2._0.Shared.FirestoreModels;
using Firestore.Controllers;

namespace BoyumFoosballStats_2._0.Services.Interface;

public interface IPlayerService : IFirestoreCrudController<Player>
{
}