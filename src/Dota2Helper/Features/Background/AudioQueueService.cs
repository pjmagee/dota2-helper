using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Dota2Helper.Features.Audio;
using Dota2Helper.Features.Settings;
using Microsoft.Extensions.Hosting;
using NAudio.Wave;

namespace Dota2Helper.Features.Background;

public class AudioQueueService(SettingsService settingsService, AudioQueue audioQueue) : BackgroundService
{
    readonly WaveOutEvent _mediaPlayer = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            while (audioQueue.TryDequeue(out var audioFile))
            {
                try
                {
                    var audioFileReader = new AudioFileReader(audioFile);

                    try
                    {
                        _mediaPlayer.Init(audioFileReader);
                        _mediaPlayer.Volume = (float)(settingsService.Settings.Volume / 100.0);
                        _mediaPlayer.Play();

                        while (_mediaPlayer.PlaybackState == PlaybackState.Playing)
                        {
                            await Task.Delay(100, stoppingToken);
                        }
                    }
                    finally
                    {
                        await audioFileReader.DisposeAsync();
                    }
                }
                catch (Exception e)
                {
                   Debug.WriteLine(e.Message);
                }
            }

            await Task.Delay(500, stoppingToken);
        }
    }

    public override void Dispose()
    {
        _mediaPlayer?.Dispose();
        base.Dispose();
    }
}