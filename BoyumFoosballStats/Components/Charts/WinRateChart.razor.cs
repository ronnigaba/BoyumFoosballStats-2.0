using System;
using System.Collections.Generic;
using BoyumFoosballStats.Components.Charts.Models;
using Microsoft.AspNetCore.Components;

namespace BoyumFoosballStats.Components.Charts;

public partial class WinRateChart : ChartComponentBase
{
    public string FormatAsPercentage(object value)
    {
        return $"{Convert.ToDouble(value) * 100:0.##}%";
    }
}
