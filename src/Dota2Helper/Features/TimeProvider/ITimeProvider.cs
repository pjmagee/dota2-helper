using System;

namespace Dota2Helper.Features.TimeProvider;

public interface ITimeProvider
{
    TimeProviderType TimeProviderType { get; }

    public TimeSpan Time { get; }
}