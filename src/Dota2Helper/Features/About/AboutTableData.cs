using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

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
            return JsonSerializer.Deserialize<List<PackageItem>>(json)!;
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
        }

        return [];
    }
}