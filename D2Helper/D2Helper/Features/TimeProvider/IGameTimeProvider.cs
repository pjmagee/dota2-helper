using System;

namespace D2Helper.Features.TimeProvider;

public interface IGameTimeProvider
{
    public TimeSpan Time { get; }
}