using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dota2Helper.Features.About;

public class AboutTableData : SortedSet<AboutItem>
{
    public AboutTableData()
    {
        foreach (var item in GetReferencedAssemblies())
        {
            Add(item);
        }
    }

    IEnumerable<AboutItem> GetReferencedAssemblies()
    {
        var currentAssembly = Assembly.GetExecutingAssembly();
        var referencedAssemblies = currentAssembly.GetReferencedAssemblies();

        List<AboutItem> items = [];

        foreach (var assemblyName in referencedAssemblies)
        {
            try
            {
                Assembly assembly = Assembly.Load(assemblyName);

                // Check if the assembly has NuGet metadata
                var hasMetaDataAttribute = HasMetaDataAttribute(assembly);

                if (!hasMetaDataAttribute) continue;

                // Retrieve description
                string description = GetAssemblyDescription(assembly);

                items.Add(new AboutItem
                    {
                        Name = assemblyName.Name!,
                        Value = assemblyName.Version!.ToString(),
                        Description = description
                    }
                );
            }
            catch
            {
                // Skip assemblies that fail to load
            }
        }

        return items;
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