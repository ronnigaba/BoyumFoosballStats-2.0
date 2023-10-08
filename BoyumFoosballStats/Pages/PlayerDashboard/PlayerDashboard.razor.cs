using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BoyumFoosballStats.Pages.PlayerDashboard.Models;
using BoyumFoosballStats.Services.Interface;
using BoyumFoosballStats.Shared.DbModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;

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