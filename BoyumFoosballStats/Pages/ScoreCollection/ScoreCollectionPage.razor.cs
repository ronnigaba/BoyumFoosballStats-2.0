using System.Threading.Tasks;
using BoyumFoosballStats.Services.Interface;
using Microsoft.AspNetCore.Components;

namespace BoyumFoosballStats.Pages.ScoreCollection;

public partial class ScoreCollectionPage
{
    [Inject] public IPlayerCrudService? PlayerService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await _ViewModel.LoadPlayers();
    }
}