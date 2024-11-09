using D2Helper.Features.Timers;

namespace D2Helper.Features.TimeProvider;

public interface ITimeProviderStrategy
{
    public TimeProviderStrategy Strategy { get; set; }
}