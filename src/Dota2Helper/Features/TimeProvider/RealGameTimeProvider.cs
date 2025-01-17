using System;

namespace Dota2Helper.Features.TimeProvider;

public class RealGameTimeProvider : ITimeProvider
{
    public TimeProviderType TimeProviderType => TimeProviderType.Real;
    public TimeSpan Time { get; set; }
}