using System.Collections.Generic;
using System.Threading.Tasks;
using BoyumFoosballStats.Pages.OverviewDashboard.Models;
using Microsoft.AspNetCore.Components;

namespace BoyumFoosballStats.Pages.OverviewDashboard;

public partial class OverviewDashboard
{
    [Inject] public IOverviewDashboardViewModel ViewModel { get; set; } = null!;


    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await ViewModel.InitializeAsync();
    }
}