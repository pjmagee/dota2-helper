using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dota2Helper.Core.Configuration;

public class TimeSpanConverter : JsonConverter<TimeSpan>
{
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();

        if (stringValue == null)
        {
            return TimeSpan.Zero;
        }

        bool isNegative = stringValue.StartsWith('-');

        if (TimeSpan.TryParseExact(stringValue.TrimStart('-'), @"mm\:ss", CultureInfo.InvariantCulture, TimeSpanStyles.None, out var timeSpan))
        {
            if (isNegative)
            {
                timeSpan = timeSpan.Negate();
            }

            return timeSpan;
        }

        throw new JsonException("Invalid TimeSpan format.");
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        if (value < TimeSpan.Zero)
        {
            writer.WriteStringValue("-" + value.ToString("mm\\:ss"));
        }
        else
        {
            writer.WriteStringValue(value.ToString("mm\\:ss"));
        }
    }
}