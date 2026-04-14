using System;
using System.Globalization;
using System.Windows.Data;

namespace WineTracker.Converters;

[ValueConversion(typeof(int), typeof(string))]
public class RatingToStarsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int rating)
        {
            int r = Math.Clamp(rating, 1, 5);
            return new string('★', r) + new string('☆', 5 - r);
        }
        return "☆☆☆☆☆";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;
}
