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

        if (Process.GetProcessesByName("dota2").Any())
        {
            logger.LogInformation("Dota 2 is running, using DotaListener");
            
            var state = await dotaListener.GetStateAsync(cancellationToken);
                
            if (state != null)
            {
                logger.LogInformation("DotaListener is working");
                return dotaListener;
            }
        }
        
        logger.LogInformation("Dota 2 is not running, using FakeDotaListener");

        return fakeDotaListener;
    }
}