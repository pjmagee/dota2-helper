using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Dota2Helper.Features.Settings;
using NAudio.Wave;

namespace Dota2Helper.Features.Audio;

public class AudioService : BackgroundWorker, IAudioService, IDisposable
{
    readonly SettingsService _settingsService;
    readonly WaveOutEvent _mediaPlayer;

    ConcurrentQueue<string> AudioQueue { get; } = new();

    public AudioService(SettingsService settingsService)
    {
        _settingsService = settingsService;
        _mediaPlayer = new WaveOutEvent();
        RunWorkerAsync();
    }

    public void Play(string audioFile)
    {
        if (!string.IsNullOrWhiteSpace(audioFile) && !AudioQueue.Contains(audioFile))
        {
            AudioQueue.Enqueue(audioFile);
        }
    }

    protected override async void OnDoWork(DoWorkEventArgs e)
    {
        while (!CancellationPending)
        {
            while (AudioQueue.TryDequeue(out var audioFile))
            {
                try
                {
                    using (var audioFileReader = new AudioFileReader(audioFile))
                    {
                        _mediaPlayer.Init(audioFileReader);
                        _mediaPlayer.Volume = (float)(_settingsService.Settings.Volume / 100.0);
                        _mediaPlayer.Play();
                    }
                }
                catch (Exception)
                {

                }
            }

            await Task.Delay(1000);
        }
    }
}