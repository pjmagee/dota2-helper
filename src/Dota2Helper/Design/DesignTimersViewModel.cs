using Dota2Helper.Features.Background;
using Dota2Helper.Features.Settings;
using Dota2Helper.Features.TimeProvider;
using Dota2Helper.ViewModels;

namespace Dota2Helper.Design;

public class DesignTimersViewModel : TimersViewModel
{
    DesignTimersViewModel(ITimeProvider timeProvider) : base(new DesignSettingsViewModel(), timeProvider)
    {

    }

    DesignTimersViewModel(SettingsService settingsService) : this(new DemoGameTimeProvider())
    {

    }

    public DesignTimersViewModel() : this(new DesignSettingsService())
    {

    }
}