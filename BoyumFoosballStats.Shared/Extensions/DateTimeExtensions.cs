namespace BoyumFoosballStats.Shared.Extensions;

public static class DateTimeExtensions
{
    public static string GetSeasonKey(this DateTime dateTime)
    {
        return dateTime.Year + GetQuarter(dateTime);
    }

    public static string GetQuarter(this DateTime datetime)
    {
        switch (datetime.Month)
        {
            case 1:
            case 2:
            case 3:
                return "Q1";
            case 4:
            case 5:
            case 6:
                return "Q2";
            case 7:
            case 8:
            case 9:
                return "Q3";
            case 10:
            case 11:
            case 12:
                return "Q4";
            default: throw new ArgumentOutOfRangeException(nameof(datetime));
        }
    }
}