using System;

namespace D2Helper.Features.TimeProvider;

public interface ITimeProvider
{
    ProviderType ProviderType { get; }

    public TimeSpan Time { get; }
}