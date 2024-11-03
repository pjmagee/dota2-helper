using System;
using System.ComponentModel;
using System.Threading;

namespace D2Helper.Services;

public class GameStateService : BackgroundWorker
{
    public event EventHandler<TimeSpan>? TimeElapsed;
    public TimeSpan Time { get; set; } = TimeSpan.Zero;

    protected override void OnDoWork(DoWorkEventArgs e)
    {
        while (!CancellationPending)
        {
            TimeElapsed?.Invoke(this, Time);
            Thread.Sleep(TimeSpan.FromSeconds(1));
            Time = Time.Add(TimeSpan.FromSeconds(2));
        }
    }
}