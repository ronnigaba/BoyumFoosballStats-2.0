﻿using System;
using System.Collections.Generic;
using System.Globalization;
using BoyumFoosballStats.Components.Charts.Models;
using BoyumFoosballStats.Shared.Extensions;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;

namespace BoyumFoosballStats.Components.Charts;

public class ChartComponentBase : ComponentBase
{
    [Parameter] public List<ChartDataItem> Data { get; set; }

    protected static string FormatAsPercentage(object value)
    {
        return $"{Convert.ToDouble(value) * 100:0.##}%";
    }

    protected string FormatAsWeekRange(object value)
    {
        if (value is not DateTime dateInWeek)
        {
            return string.Empty;
        }
        
        var weekStart = dateInWeek.StartOfWeek(DayOfWeek.Monday);
        var weekEnd = weekStart.AddDays(4); // Adding 4 days to get to Friday

        return $"{weekStart:MMMM dd}-{weekEnd:dd}";
    }
    protected string FormatAsWeekNumber(object value)
    {
        if (value is not DateTime dateInWeek)
        {
            return string.Empty;
        }

        return CultureInfo.CurrentCulture.Calendar
            .GetWeekOfYear(dateInWeek, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday).ToString("00");
    }
    
    protected string FormatAsShortDayName(object value)
    {
        if (value is not DateTime dateInWeek)
        {
            return string.Empty;
        }

        return dateInWeek.DayOfWeek.ToString().Substring(0,3);
    }    
    protected string FormatAsFullDayName(object value)
    {
        if (value is not DateTime dateInWeek)
        {
            return string.Empty;
        }

        return dateInWeek.DayOfWeek.ToString();
    }
    
    protected string FormatAsTrueSkill(object value)
    {
        if (value is not double number)
        {
            return string.Empty;
        }
        
        return number.ToTrueSkillString();
    }
}