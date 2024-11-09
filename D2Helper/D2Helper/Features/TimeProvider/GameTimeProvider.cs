using System;

namespace D2Helper.Features.TimeProvider;

public class GameTimeProvider : IGameTimeProvider
{
    public TimeSpan Time { get; set; }
}