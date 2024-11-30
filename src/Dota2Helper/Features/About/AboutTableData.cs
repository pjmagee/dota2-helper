using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Dota2Helper.Features.About;

public class AboutTableData : SortedSet<PackageItem>
{
    public AboutTableData()
    {
        foreach (var item in GetPackageItems())
        {
            Add(item);
        }
    }

    IEnumerable<PackageItem> GetPackageItems()
    {
        try
        {
            var dir = Directory.GetCurrentDirectory();
            var json = File.ReadAllText(Path.Combine(dir, "packages.json"));
            return System.Text.Json.JsonSerializer.Deserialize<List<PackageItem>>(json)!;
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
        }

        return [];
    }

    bool HasMetaDataAttribute(Assembly assembly)
    {
        var metadataAttributes = assembly.GetCustomAttributes<AssemblyMetadataAttribute>();
        return metadataAttributes.Any();
    }

    string GetAssemblyDescription(Assembly assembly)
    {
        var descriptionAttribute = assembly
            .GetCustomAttributes<AssemblyDescriptionAttribute>()
            .FirstOrDefault();

        return descriptionAttribute?.Description ?? "No description available";
    }
}