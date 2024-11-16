using System;

namespace Dota2Helper.Features.TimeProvider;

public interface ITimeProvider
{
    ProviderType ProviderType { get; }

    public TimeSpan Time { get; }
}