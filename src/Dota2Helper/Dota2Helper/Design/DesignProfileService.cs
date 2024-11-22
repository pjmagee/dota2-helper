using Dota2Helper.Features.Audio;
using Dota2Helper.Features.Settings;
using Dota2Helper.Features.Timers;

namespace Dota2Helper.Design;

public class DesignProfileService() : ProfileService(new SettingsService(), new ViewModelFactory(new FakeTimerAudioService()))
{
    
}