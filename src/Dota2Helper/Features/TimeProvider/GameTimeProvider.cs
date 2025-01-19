using System;

namespace Dota2Helper.Features.TimeProvider;

public sealed class GameTimeProvider : ITimeProvider
{
    public ITimeProvider? Current { get; set; }
    public TimeProviderType TimeProviderType => Current?.TimeProviderType ?? TimeProviderType.Real;
    public TimeSpan Time => Current?.Time ?? TimeSpan.Zero;
}