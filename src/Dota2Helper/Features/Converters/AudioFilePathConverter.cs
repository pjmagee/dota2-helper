using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Avalonia.Data.Converters;

namespace Dota2Helper.Features.Converters;


public class AudioFilePathConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string path && !string.IsNullOrEmpty(path))
        {
            var fi = new FileInfo(path);
            var pathParts = new List<string>
            {
                fi.Name
            };

            var di = fi.Directory;

            for (int level = 0; level < 3; level++)
            {
                if (di != null)
                {
                    pathParts.Add(di.Name);
                    di = di.Parent;
                    level++;
                }
                else
                {
                    break;
                }
            }

            pathParts.Reverse();

            return string.Join(Path.DirectorySeparatorChar, pathParts);
        }

        return value;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}