using Dota2Helper.Features.Settings;
using Dota2Helper.Features.TimeProvider;
using Dota2Helper.ViewModels;

namespace Dota2Helper.Design;

public class DesignTimersViewModel : TimersViewModel
{
    DesignTimersViewModel(GameTimeProvider timeProvider) : base(new DesignSettingsViewModel(), timeProvider)
    {

    }

    DesignTimersViewModel(SettingsService settingsService) : this(new GameTimeProvider(settingsService, new RealGameTimeProvider(), new DemoTimeProvider()))
    {

    }

    public DesignTimersViewModel() : this(new DesignSettingsService())
    {

    }
}