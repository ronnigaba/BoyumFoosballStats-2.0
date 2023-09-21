﻿using BoyumFoosballStats_2._0.Shared.FirestoreModels;
using BoyumFoosballStats.Models;

namespace BoyumFoosballStats_2._0.Pages.ScoreCollection;

public interface IScoreCollectionViewModel : IViewModelBase
{
    public bool DrawerOpen { get; set; }
    bool ShowInactivePlayers { get; set; }
    bool AutoBalanceMatches { get; set; }
    bool AutoSwapPlayers { get; set; }
    IEnumerable<Player> AvailablePlayers { get; set; }
    IEnumerable<Player>? SelectedPlayers { get; set; }
    void ToggleDrawer();
}