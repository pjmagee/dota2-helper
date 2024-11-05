using D2Helper.Services;

namespace D2Helper.ViewModels;

public class DesignTimersViewModel(TimerService timerService, DynamicProvider dynamicProvider) : TimersViewModel(timerService, dynamicProvider, dynamicProvider)
{
    public DesignTimersViewModel() : this(new TimerService(), new DynamicProvider(new RealGameTimeProvider(), new DemoGameTimeProvider()))
    {

    }
}