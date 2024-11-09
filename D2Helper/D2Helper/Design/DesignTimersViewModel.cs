using D2Helper.Features.TimeProvider;
using D2Helper.Features.Timers;
using D2Helper.ViewModels;

namespace D2Helper.Design;

public class DesignTimersViewModel : TimersViewModel
{

    public DesignTimersViewModel(TimerService timerService, StrategyTimeProvider strategyTimeProvider) : base(timerService, strategyTimeProvider, strategyTimeProvider)
    {

    }

    private DesignTimersViewModel(DesignSettingsService settingsService) : this(new TimerService(settingsService), new StrategyTimeProvider(new GameTimeProvider(), new DemoTimeProvider()))
    {

    }

    public DesignTimersViewModel() : this(new DesignSettingsService())
    {

    }
}