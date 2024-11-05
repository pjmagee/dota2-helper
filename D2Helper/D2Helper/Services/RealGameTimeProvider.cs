using System;

namespace D2Helper.Services;

public class RealGameTimeProvider : IGameTimeProvider
{
    public TimeSpan Time { get; set; }
}