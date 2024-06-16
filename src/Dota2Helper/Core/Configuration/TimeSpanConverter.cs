using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dota2Helper.Core.Configuration;

public class TimeSpanConverter  : JsonConverter<TimeSpan>
{
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();
        
        if (TimeSpan.TryParseExact(stringValue, @"mm\:ss", CultureInfo.InvariantCulture, TimeSpanStyles.None, out var timeSpan))
        {
            return timeSpan;
        }
        
        throw new JsonException("Invalid TimeSpan format.");
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("mm\\:ss"));
    }
}