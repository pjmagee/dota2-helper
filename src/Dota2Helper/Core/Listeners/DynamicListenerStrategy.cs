using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Dota2Helper.Core.Listeners;

public class DynamicListenerStrategy(
    ILogger<DynamicListenerStrategy> logger,
    FakeDotaListener fakeDotaListener,
    DotaListener dotaListener) : IListenerStrategy
{
    IDotaListener? _listener;
    Process? _dota2;

    public IDotaListener Listener => _listener ?? fakeDotaListener;

    public void UpdateListener()
    {
        if (_dota2 == null)
        {
            _dota2 = Process.GetProcessesByName("dota2").FirstOrDefault();

            if (_dota2 != null)
            {
                logger.LogInformation("Dota2 is running, caching detected process for exit event");

                _dota2.EnableRaisingEvents = true;

                _dota2.Exited += (sender, args) =>
                {
                    logger.LogInformation("Dota2 exited, switching to FakeDotaListener");

                    _listener = fakeDotaListener;
                    _dota2 = null;
                };
            }
        }

        if (_dota2 != null)
        {
            logger.LogInformation("Dota2 is running, using DotaListener");
            _listener = dotaListener;
        }
        else
        {
            logger.LogInformation("Dota2 is not running, using FakeDotaListener");
            _listener = fakeDotaListener;
        }
    }
}