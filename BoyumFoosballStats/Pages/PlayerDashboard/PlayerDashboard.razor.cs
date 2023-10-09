using System.Threading.Tasks;
using BoyumFoosballStats.Pages.PlayerDashboard.Models;
using Microsoft.AspNetCore.Components;

namespace BoyumFoosballStats.Pages.PlayerDashboard;

public partial class PlayerDashboard
{
    [Inject] public IPlayerDashboardViewModel ViewModel { get; set; } = null!;

    [Parameter]
    public string? PlayerId
    {
        get => ViewModel.PlayerId;
        set => ViewModel.PlayerId = value;
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await ViewModel.InitializeAsync();
    }
}