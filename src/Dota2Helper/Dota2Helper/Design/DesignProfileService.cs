using Dota2Helper.Features.Audio;
using Dota2Helper.Features.Timers;

namespace Dota2Helper.Design;

public class DesignProfileService() : ProfileService(new DesignSettingsService(), new ViewModelFactory(new FakeTimerAudioService()))
{
    
}