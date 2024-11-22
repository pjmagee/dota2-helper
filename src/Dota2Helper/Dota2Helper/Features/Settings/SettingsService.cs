using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Dota2Helper.Features.Settings;

public class SettingsService
{
    readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public SettingsService(IOptions<Settings> settings, IOptions<List<DotaTimer>> defaultTimers)
    {
        Settings = settings.Value;
        DefaultTimers = defaultTimers.Value;
    }

    public SettingsService()
    {
        Settings = new Settings();
        DefaultTimers = new List<DotaTimer>();
    }

    public Settings Settings { get; protected set; }

    public List<DotaTimer> DefaultTimers { get; protected set; }

    public void SaveSettings()
    {
        var json = JsonSerializer.Serialize(Settings, _jsonOptions);
        File.WriteAllText("appsettings.json", json);
    }

    public void UpdateSettings(Settings updatedSettings)
    {
        Settings = updatedSettings;
        SaveSettings();
    }
}