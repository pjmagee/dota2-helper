using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dota2Helper.Features.Settings;
using Microsoft.Extensions.Hosting;
using NAudio.Wave;

namespace Dota2Helper.Features.Audio;

public class AudioService(SettingsService settingsService) : BackgroundService, IAudioService
{
    private readonly WaveOutEvent _mediaPlayer = new();
    private ConcurrentQueue<string> AudioQueue { get; } = new();

    public void Play(string audioFile)
    {
        if (!string.IsNullOrWhiteSpace(audioFile) && !AudioQueue.Contains(audioFile))
        {
            AudioQueue.Enqueue(audioFile);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            while (AudioQueue.TryDequeue(out var audioFile))
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

            await Task.Delay(1000, stoppingToken);
        }
    }

    public override void Dispose()
    {
        _mediaPlayer?.Dispose();
        base.Dispose();
    }
}