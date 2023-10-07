using System.Threading.Tasks;
using BoyumFoosballStats.Shared.DbModels;
using Microsoft.AspNetCore.Components;

namespace BoyumFoosballStats.Components.PlayerCard;

public partial class PlayerCard
{
    [Parameter] public Player? SelectedPlayer { get; set; }
    [Parameter] public EventCallback<Player?> SelectedPlayerChanged { get; set; }
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private async Task HandleClosePlayerCard()
    {
        SelectedPlayer = null;
        await SelectedPlayerChanged.InvokeAsync(SelectedPlayer);
    }
}