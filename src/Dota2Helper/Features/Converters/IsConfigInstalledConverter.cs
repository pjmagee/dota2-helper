using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Dota2Helper.Features.Converters;

public class IsConfigInstalledConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isConfigInstalled)
        {
            return isConfigInstalled ? "Uninstall" : "Install";
        }

        return value;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}