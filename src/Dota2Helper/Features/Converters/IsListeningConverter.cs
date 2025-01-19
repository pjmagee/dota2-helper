using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Dota2Helper.Features.Converters;

public class IsListeningConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isListening)
        {
            return isListening ? "Stop" : "Start";
        }

        return value;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}