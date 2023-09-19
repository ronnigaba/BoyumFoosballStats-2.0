using BoyumFoosballStats_2._0.Services.Interface;
using Microsoft.AspNetCore.Components;

namespace BoyumFoosballStats_2._0.Pages.ScoreCollection;

public partial class ScoreCollectionPage
{
    [Inject] public IPlayerService? PlayerService { get; set; }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && PlayerService != null) 
        {
            var players = await PlayerService.ReadAllAsync();
        }
        await base.OnAfterRenderAsync(firstRender);
    }
}