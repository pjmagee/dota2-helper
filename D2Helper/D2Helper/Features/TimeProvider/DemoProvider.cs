using System;
using System.ComponentModel;
using System.Threading;

namespace D2Helper.Features.TimeProvider;

public class DemoProvider : BackgroundWorker, ITimeProvider
{
    public ProviderType ProviderType => ProviderType.Demo;
    public TimeSpan Time { get; private set; }

    public DemoProvider()
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
}