using System.Threading.Tasks;
using BoyumFoosballStats.Cosmos.Services.Interfaces;
using BoyumFoosballStats.Pages.ScoreCollection.Models;
using BoyumFoosballStats.Services.Interface;
using Microsoft.AspNetCore.Components;

namespace BoyumFoosballStats.Pages.ScoreCollection;

public partial class ScoreCollectionPage
{
    [Inject] public IPlayerCrudService? PlayerService { get; set; }

    [Inject] public IScoreCollectionViewModel ViewModel { get; set; } = null!;
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await ViewModel.LoadPlayers();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ViewModel.LoadSession();
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }
}