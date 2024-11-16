using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Dota2Helper.Features.Settings;

public class SettingsService
{
    readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public Settings Settings { get; protected set; }

    public List<DotaTimer> DefaultTimers { get; set; }

    public SettingsService()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.timers.default.json", optional: true)
            .Build();

        Settings = configuration.Get<Settings>()!;
        DefaultTimers = configuration.GetSection("DefaultTimers").Get<List<DotaTimer>>()!;
    }

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