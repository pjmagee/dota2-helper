using System;
using System.ComponentModel;
using System.Threading;

namespace Dota2Helper.Features.TimeProvider;

public class DemoProvider : BackgroundWorker, ITimeProvider
{
    public ProviderType ProviderType => ProviderType.Demo;
    public TimeSpan Time { get; private set; } = TimeSpan.FromSeconds(-90);

    public DemoProvider()
    {
        RunWorkerAsync();
    }

    protected override void OnDoWork(DoWorkEventArgs e)
    {
        while (!CancellationPending)
        {
            if (Time > TimeSpan.FromMinutes(60))
            {
                Time = TimeSpan.FromSeconds(-90);
            }

            Time += TimeSpan.FromSeconds(1);
            Thread.Sleep(500);
        }
    }
}