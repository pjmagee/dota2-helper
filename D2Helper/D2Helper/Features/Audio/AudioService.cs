using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using D2Helper.Features.Settings;
using D2Helper.Features.TimeProvider;
using D2Helper.Features.Timers;
using LibVLCSharp.Shared;

namespace D2Helper.Features.Audio;

public class AudioService : BackgroundWorker
{
    LibVLC LibVlc { get; } = new();
    readonly MediaPlayer _mediaPlayer;

    Queue<string> AudioQueue { get; } = new();

    public AudioService()
    {
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