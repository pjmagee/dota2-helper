using D2Helper.Services;
using D2Helper.ViewModels;

namespace D2Helper.Design;

public class DesignTimersViewModel : TimersViewModel
{

    public DesignTimersViewModel(TimerService timerService, DynamicProvider dynamicProvider) : base(timerService, dynamicProvider, dynamicProvider)
    {

    }

    private DesignTimersViewModel(DesignSettingsService settingsService) : this(new TimerService(settingsService), new DynamicProvider(new RealGameTimeProvider(), new DemoGameTimeProvider()))
    {

    }

    public DesignTimersViewModel() : this(new DesignSettingsService())
    {

    }
}