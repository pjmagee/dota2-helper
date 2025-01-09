using System;

namespace Dota2Helper.Features.TimeProvider;

public class RealGameTimeProvider : ITimeProvider
{
    public ProviderType ProviderType => ProviderType.Real;
    public TimeSpan Time { get; set; }
}