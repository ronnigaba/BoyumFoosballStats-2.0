using System.Threading.Tasks;
using BoyumFoosballStats.Pages.MatchHistory.Models;
using Microsoft.AspNetCore.Components;

namespace BoyumFoosballStats.Pages.MatchHistory;

public partial class MatchHistory
{
    [Inject] public IMatchHistoryViewModel ViewModel { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await ViewModel.InitializeAsync();
    }
}