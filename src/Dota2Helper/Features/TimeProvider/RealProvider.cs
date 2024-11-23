using System;

namespace Dota2Helper.Features.TimeProvider;

public class RealProvider : ITimeProvider
{
    public ProviderType ProviderType => ProviderType.Real;
    public TimeSpan Time { get; set; }
}