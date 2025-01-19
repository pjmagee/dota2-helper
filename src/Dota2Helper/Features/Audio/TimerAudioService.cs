using Dota2Helper.Features.Settings;
using Dota2Helper.Features.TimeProvider;
using Microsoft.Extensions.DependencyInjection;

namespace Dota2Helper.Features.Audio;

public class TimerAudioService(ITimeProvider timeProvider, SettingsService settingsService, [FromKeyedServices(nameof(AudioService))] IAudioService audioService) : IAudioService
{
    public void Play(string audioFile)
    {
        switch (timeProvider.TimeProviderType)
        {
            case TimeProviderType.Real:
            case TimeProviderType.Demo when !settingsService.Settings.DemoMuted:
                audioService.Play(audioFile);
                break;
        }
    }
}