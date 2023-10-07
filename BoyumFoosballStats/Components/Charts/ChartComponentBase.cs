using System.Collections.Generic;
using BoyumFoosballStats.Components.Charts.Models;
using Microsoft.AspNetCore.Components;

namespace BoyumFoosballStats.Components.Charts;

public class ChartComponentBase : ComponentBase
{
    [Parameter] 
    public List<ChartDataItem> Data { get; set; }
}