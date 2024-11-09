using System.IO;

namespace D2Helper.Features.Gsi;

public sealed class GsiConfigWatcher : FileSystemWatcher
{
    public GsiConfigWatcher(GsiConfigService configurationService)
    {
        Path = configurationService.GetGameStateIntegrationPath() ?? "";
        NotifyFilter = NotifyFilters.LastWrite;
        Filter = "*.cfg";
        EnableRaisingEvents = true;
    }
}