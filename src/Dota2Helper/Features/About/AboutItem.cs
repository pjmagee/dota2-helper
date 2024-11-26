using System;

namespace Dota2Helper.Features.About;

public class AboutItem : IComparable<AboutItem>
{
    public int CompareTo(AboutItem? other)
    {
        return string.Compare(Name, other?.Name, StringComparison.Ordinal);
    }

    public required string Name { get; set; }
    public required string Value { get; set; }
    public required string Description { get; set; }
}