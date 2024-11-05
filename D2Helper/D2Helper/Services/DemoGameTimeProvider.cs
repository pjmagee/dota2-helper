using System;
using System.ComponentModel;
using System.Threading;

namespace D2Helper.Services;

public class DemoGameTimeProvider : BackgroundWorker, IGameTimeProvider
{
    public DemoGameTimeProvider()
    {
        RunWorkerAsync();
    }

    protected override void OnDoWork(DoWorkEventArgs e)
    {
        while (!CancellationPending)
        {
            Time += TimeSpan.FromSeconds(1);
            Thread.Sleep(1000);
        }
    }

    public TimeSpan Time { get; private set; }
}