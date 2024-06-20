using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Microsoft.Extensions.Logging;

namespace Dota2Helper.Core.Listeners;

public class DynamicListenerStrategy(
    ILogger<DynamicListenerStrategy> logger,
    FakeDotaListener fakeDotaListener, 
    DotaListener dotaListener) : IListenerStrategy
{
    public async Task<IDotaListener> GetListener(CancellationToken cancellationToken)
    {
        if (Design.IsDesignMode)
        {
            return fakeDotaListener;
        }

        var dota2 = Process.GetProcessesByName("dota2").FirstOrDefault();
       
        if (dota2 is not null)
        {
            logger.LogInformation("Dota2 is running, using DotaListener");
            return dotaListener;
        }

        logger.LogInformation("Dota2 is not running, using FakeListener");
        return fakeDotaListener;
    }
}