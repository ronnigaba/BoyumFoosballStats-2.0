using System;

namespace BoyumFoosballStats.Extensions;

public static class ObjectExtensions
{
    public static string ToPercentageString(this object value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value), "Value cannot be null.");
        }

        if (value is IConvertible convertibleValue)
        {
            double numericValue = convertibleValue.ToDouble(System.Globalization.CultureInfo.InvariantCulture);
            
            if (!double.IsNaN(numericValue))
            {
                return $"{numericValue:P}";
            }
            else
            {
                throw new InvalidCastException("Value cannot be converted to a numeric type.");
            }
        }
        else
        {
            throw new InvalidCastException("Value does not implement IConvertible.");
        }
    }
}