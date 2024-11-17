using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Dota2Helper.Features.Settings;
using LibVLCSharp.Shared;

namespace Dota2Helper.Features.Audio;

public interface IAudioService
{
    void Play(string audioFile);
}

public class AudioService : BackgroundWorker, IAudioService
{
    readonly SettingsService _settingsService;
    LibVLC LibVlc { get; } = new();
    readonly MediaPlayer _mediaPlayer;

    Queue<string> AudioQueue { get; } = new();

    public AudioService(SettingsService settingsService)
    {
        _settingsService = settingsService;
        _mediaPlayer = new MediaPlayer(LibVlc);
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
                    var uri = new Uri(audioFile, UriKind.RelativeOrAbsolute);
                    var fromType = uri.IsAbsoluteUri ? FromType.FromLocation : FromType.FromPath;

                    using(var media = new Media(LibVlc, audioFile, fromType))
                    {
                        _mediaPlayer.Volume = (int) _settingsService.Settings.Volume;
                        _mediaPlayer.Play(media);
                    }
                }
                catch(Exception)
                {

                }
            }

            await Task.Delay(1000);
        }
    }
}