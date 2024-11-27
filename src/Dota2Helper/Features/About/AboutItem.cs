using System;
using System.Text.Json.Serialization;

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
    public string PackageProjectUrl { get; set; }

    [JsonPropertyName("Copyright")]
    public string Copyright { get; set; }

    [JsonPropertyName("Authors")]
    public string Authors { get; set; }

    [JsonPropertyName("License")]
    public string License { get; set; }

    [JsonPropertyName("LicenseUrl")]
    public string LicenseUrl { get; set; }
}