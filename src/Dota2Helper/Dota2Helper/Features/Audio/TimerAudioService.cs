using Dota2Helper.Features.Settings;
using Dota2Helper.Features.TimeProvider;

namespace Dota2Helper.Features.Audio;

public class TimerAudioService(
    ITimeProvider timeProvider,
    SettingsService settingsService,
    AudioService audioService)
{
    public void Play(string audioFile)
    {
        if (timeProvider.ProviderType == ProviderType.Real)
        {
            audioService.Play(audioFile);
        }
        else if (timeProvider.ProviderType == ProviderType.Demo && !settingsService.Settings.DemoMuted)
        {
            audioService.Play(audioFile);
        }
    }
}