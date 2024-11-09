namespace D2Helper.Features.Timers;

public class TimerStrategy
{
    public required string Name { get; set; }
    public TimeProviderStrategy Strategy { get; set; }
}