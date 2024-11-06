using System.IO;
using System.Text.Json;
using D2Helper.Models;
using Microsoft.Extensions.Configuration;

namespace D2Helper.Services;

public class SettingsService
{
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { WriteIndented = true };

    public Settings Settings { get; protected set; }

    public SettingsService()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        Settings = configuration.Get<Settings>()!;
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