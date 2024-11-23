using System;
using System.Globalization;
using System.IO;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;
using Dota2Helper.ViewModels;

namespace Dota2Helper.Features.Audio;

public class AudioFilePathConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string path && !string.IsNullOrEmpty(path))
        {
            var fileName = Path.GetFileName(path);
            var parentDir = Path.GetFileName(Path.GetDirectoryName(path));
            value = $"..\\{parentDir}\\{fileName}";
        }

        return value;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}