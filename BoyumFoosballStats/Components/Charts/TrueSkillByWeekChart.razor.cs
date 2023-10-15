using System.Collections.Generic;
using BoyumFoosballStats.Components.Charts.Models;
using Microsoft.AspNetCore.Components;

namespace BoyumFoosballStats.Components.Charts;

public partial class TrueSkillByWeekChart : ChartComponentBase
{
    [Parameter] public List<ChartDataItem> SecondaryData { get; set; }
}