using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using ValveKeyValue;
using static Avalonia.Controls.Design;

namespace Dota2Helper.Features.Gsi;

public partial class GsiConfigService
{
    private const string GsiFlag = "-gamestateintegration";

    string ConfigFile => IsDesignMode ? "gamestate_integration_design.cfg" : "gamestate_integration_d2helper.cfg";

    public bool IsLaunchArgumentPresent()
    {
        var steamInstallationPath = GetSteamInstallationPath();
        if (steamInstallationPath == null) return false;

        var localConfigPath = Path.Combine(steamInstallationPath, "userdata");
        var configs = Directory.EnumerateFileSystemEntries(localConfigPath, "localconfig.vdf", SearchOption.AllDirectories);

        foreach (var config in configs)
        {
            try
            {
                using (var stream = File.Open(config, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var kv = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
                    var localConfig = kv.Deserialize(stream, new KVSerializerOptions()
                        {
                            HasEscapeSequences = true,
                            EnableValveNullByteBugBehavior = true
                        }
                    );

                    string launchOptions = localConfig["Software"]["Valve"]["Steam"]["apps"]["570"]["LaunchOptions"].ToString(CultureInfo.InvariantCulture);

                    if (!string.IsNullOrWhiteSpace(launchOptions) && launchOptions.Contains(GsiFlag))
                    {
                        return true;
                    }

                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        return false;
    }


    [GeneratedRegex(@"\s*\""path\""\s*\""(.+?)\""", RegexOptions.Compiled)]
    private static partial Regex LibraryLocations();

    public string? ConfigFilePath
    {
        get
        {
            var gameStateIntegrationPath = GetGameStateIntegrationPath();
            return gameStateIntegrationPath == null ? null : Path.Combine(gameStateIntegrationPath, ConfigFile);
        }
    }

    public string? GetGameStateIntegrationPath()
    {
        var dota2FolderPath = GetDota2InstallationPath();

        if (dota2FolderPath == null) return null;

        var gameStateIntegrationPath = Path.Combine(dota2FolderPath, "game", "dota", "cfg", "gamestate_integration");
        return Directory.Exists(gameStateIntegrationPath) ? gameStateIntegrationPath : null;
    }

    public int? GetPortNumber()
    {
        var gsiPath = GetGameStateIntegrationPath();
        if (gsiPath == null) return null;

        var gsiCfgPath = Path.Combine(gsiPath, ConfigFile);
        if (!File.Exists(gsiCfgPath)) return null;

        // read error issue
        // var rawConfig = File.ReadAllText(gsiCfgPath);

        string? rawConfig = null;

        using (var stream = File.Open(gsiCfgPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            using (var reader = new StreamReader(stream))
            {
                rawConfig = reader.ReadToEnd();
            }
        }

        var match = Regex.Match(rawConfig, @"""uri""\s*""http://localhost:(\d+)/""");
        if (!match.Success) return null;

        return int.Parse(match.Groups[1].Value);
    }

    public Uri GetUri()
    {
        var portNumber = GetPortNumber();
        return portNumber == null ? new Uri("http://localhost:4001/") : new Uri($"http://localhost:{portNumber}/");
    }

    public bool TryInstall()
    {
        if (IsIntegrationInstalled()) return false;

        var dota2FolderPath = GetDota2InstallationPath();
        if (dota2FolderPath == null) return false;

        var gameStateIntegrationPath = Path.Combine(dota2FolderPath, "game", "dota", "cfg", "gamestate_integration");
        if (!Directory.Exists(gameStateIntegrationPath)) Directory.CreateDirectory(gameStateIntegrationPath);

        Install();
        return true;
    }

    public bool IsIntegrationInstalled()
    {
        var gameStateIntegrationPath = GetGameStateIntegrationPath();

        if (gameStateIntegrationPath == null) return false;

        var configFileFullPath = Path.Combine(gameStateIntegrationPath, ConfigFile);
        return File.Exists(configFileFullPath);
    }

    string? GetDota2InstallationPath()
    {
        var libraryFoldersPath = GetLibraryFoldersPath();

        if (string.IsNullOrEmpty(libraryFoldersPath) || !File.Exists(libraryFoldersPath))
        {
            return null;
        }

        var steamInstallPath = GetSteamInstallationPath();

        if (string.IsNullOrEmpty(steamInstallPath))
        {
            return null;
        }

        var steamLibraryPaths = GetSteamLibraryPaths(libraryFoldersPath);

        foreach (var steamLibraryPath in steamLibraryPaths)
        {
            var dota2FolderPath = Path.Combine(steamLibraryPath, "steamapps", "common", "dota 2 beta");

            if (Directory.Exists(dota2FolderPath)) return dota2FolderPath;
        }

        return null;
    }

    string? GetSteamInstallationPath()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? GetWindowsInstall() : RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? GetMacInstall() : null;
    }

    string? GetLibraryFoldersPath()
    {
        var steamInstallPath = GetSteamInstallationPath();
        return steamInstallPath == null ? null : Path.Combine(steamInstallPath, "steamapps", "libraryfolders.vdf");
    }

    [SupportedOSPlatform("windows")]
    string? GetWindowsInstall()
    {
        const string steamRegistryKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam";
        const string steamRegistryValue = "InstallPath";

        try
        {
            var installPath = Registry.GetValue(steamRegistryKey, steamRegistryValue, null);
            if (installPath != null) return installPath.ToString();
        }
        catch (Exception)
        {

        }

        return null;
    }

    [SupportedOSPlatform("macos")]
    string? GetMacInstall() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Application Support", "Steam");

    string[] GetSteamLibraryPaths(string libraryFoldersVdfPath)
    {
        var pathRegex = LibraryLocations();
        var fileContent = File.ReadAllText(libraryFoldersVdfPath);
        var matches = pathRegex.Matches(fileContent);
        var libraryPaths = matches.Select(m => m.Groups[1].Value.Replace("\\\\", "\\")).ToList();
        return libraryPaths.Where(Directory.Exists).ToArray();
    }

    public void Uninstall()
    {
        var gameStateIntegrationPath = GetGameStateIntegrationPath();
        if (gameStateIntegrationPath == null) return;
        var configFileFullPath = Path.Combine(gameStateIntegrationPath, ConfigFile);
        if (File.Exists(configFileFullPath)) File.Delete(configFileFullPath);
    }

    public void Install()
    {
        var gameStateIntegrationPath = GetGameStateIntegrationPath();
        if (gameStateIntegrationPath == null) return;
        var configFileFullPath = Path.Combine(gameStateIntegrationPath, ConfigFile);
        var configFileSourcePath = Path.Combine(Directory.GetCurrentDirectory(), ConfigFile);
        if (File.Exists(configFileFullPath)) File.Delete(configFileFullPath);
        File.Copy(configFileSourcePath, configFileFullPath);
    }

    public void OpenGsiFolder()
    {
        var gameStateIntegrationPath = GetGameStateIntegrationPath();
        if (gameStateIntegrationPath == null) return;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Process.Start("explorer.exe", gameStateIntegrationPath);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("open", gameStateIntegrationPath);
        }
    }
}