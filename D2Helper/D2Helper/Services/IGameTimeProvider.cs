using System;

namespace D2Helper.Services;

public interface IGameTimeProvider
{
    public TimeSpan Time { get; }
}