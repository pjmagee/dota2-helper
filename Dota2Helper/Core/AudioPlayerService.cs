using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Dota2Helper.Core;

public class AudioPlayerService(AudioPlayer audioPlayer) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (audioPlayer.HasAudioToPlay)
            {
                try
                {
                    audioPlayer.PlayReminder();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            
            await Task.Delay(1000, stoppingToken);
        }
    }
}