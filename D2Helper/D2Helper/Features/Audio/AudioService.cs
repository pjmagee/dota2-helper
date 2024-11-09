using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using LibVLCSharp.Shared;

namespace D2Helper.Features.Audio;

public class AudioService : BackgroundWorker
{
    LibVLC LibVlc { get; } = new();
    readonly MediaPlayer _mediaPlayer;

    Queue<string> AudioQueue { get; } = new();

    public AudioService()
    {
        RunWorkerAsync();
        _mediaPlayer = new MediaPlayer(LibVlc);
    }

    public void Queue(string audioFile)
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
                    using var media = new Media(LibVlc, audioFile, FromType.FromLocation);
                    {
                        _mediaPlayer.Play(media);
                    }
                }
                catch
                {

                }
            }

            await Task.Delay(1000);
        }
    }
}