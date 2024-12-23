using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dota2Helper.Features.Settings;

namespace Dota2Helper.Features.About;

public class PackageItem : IComparable<PackageItem>
{
    public int CompareTo(PackageItem? other)
    {
        return string.Compare(PackageId, other?.PackageId, StringComparison.Ordinal);
    }

    [JsonPropertyName("PackageId")]
    public string PackageId { get; set; }

    [JsonPropertyName("PackageVersion")]
    public string PackageVersion { get; set; }

    [JsonPropertyName("PackageProjectUrl")]
    string PackageProjectUrl { get; set; }

    [JsonPropertyName("Copyright")]
    string Copyright { get; set; }

    [JsonPropertyName("Authors")]
    public string Authors { get; set; }

    [JsonPropertyName("License")]
    public string License { get; set; }

    [JsonPropertyName("LicenseUrl")]
    public string LicenseUrl { get; set; }
}