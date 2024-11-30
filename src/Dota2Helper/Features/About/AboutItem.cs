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

    [SetsRequiredMembers]
    public PackageItem(string packageId, string packageVersion, string packageProjectUrl, string copyright, string authors, string license, string licenseUrl)
    {
        PackageId = packageId;
        PackageVersion = packageVersion;
        PackageProjectUrl = packageProjectUrl;
        Copyright = copyright;
        Authors = authors;
        License = license;
        LicenseUrl = licenseUrl;
    }

    [JsonPropertyName("PackageId")]
    public required string PackageId { get; set; }

    [JsonPropertyName("PackageVersion")]
    public required string PackageVersion { get; set; }

    [JsonPropertyName("PackageProjectUrl")]
    public required string PackageProjectUrl { get; set; }

    [JsonPropertyName("Copyright")]
    public required string Copyright { get; set; }

    [JsonPropertyName("Authors")]
    public required string Authors { get; set; }

    [JsonPropertyName("License")]
    public required string License { get; set; }

    [JsonPropertyName("LicenseUrl")]
    public required string LicenseUrl { get; set; }
}