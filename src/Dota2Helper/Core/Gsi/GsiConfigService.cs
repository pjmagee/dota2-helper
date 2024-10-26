using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace Dota2Helper.Core.Gsi;

public partial class GsiConfigService(ILogger<GsiConfigService> logger)
{
    const string ConfigFile = "gamestate_integration_d2helper.cfg";

    string? FindGameStateIntegrationPath()
    {
        var dota2FolderPath = FindDota2InstallationPath();

        if (dota2FolderPath == null) return null;

        var gameStateIntegrationPath = Path.Combine(dota2FolderPath, "game", "dota", "cfg", "gamestate_integration");
        return Directory.Exists(gameStateIntegrationPath) ? gameStateIntegrationPath : null;
    }

    public int? GetPortNumber()
    {
        var gsiPath = FindGameStateIntegrationPath();
        if (gsiPath == null) return null;

        var gsiCfgPath = Path.Combine(gsiPath, ConfigFile);
        if (!File.Exists(gsiCfgPath)) return null;

        var rawConfig = File.ReadAllText(gsiCfgPath);
        var match = Regex.Match(rawConfig, @"""uri""\s*""http://localhost:(\d+)/""");
        if (!match.Success) return null;

        return int.Parse(match.Groups[1].Value);
    }

    public Uri GetUri()
    {
        var portNumber = GetPortNumber();
        return portNumber == null ? new Uri("http://localhost:4001/") : new Uri($"http://localhost:{portNumber}/");
    }

    public void SetPortNumber(int portNumber)
    {
        var gsiPath = FindGameStateIntegrationPath();

        if (gsiPath == null)
        {
            logger.LogError("gamestate_integration folder not found");
            return;
        }

        var gsiCfgPath = Path.Combine(gsiPath, ConfigFile);

        if (!File.Exists(gsiCfgPath))
        {
            logger.LogError("gamestate_integration config file not found");
            return;
        }

        var rawContents = File.ReadAllText(gsiCfgPath);
        var newConfigFileContent = Regex.Replace(rawContents, @"""uri""\s*""http://localhost:\d+/""", $@"""uri"" ""http://localhost:{portNumber}/""");

        File.WriteAllText(gsiCfgPath, newConfigFileContent);
    }

    public bool IsIntegrationInstalled()
    {
        var gameStateIntegrationPath = FindGameStateIntegrationPath();

        if (gameStateIntegrationPath == null) return false;

        var configFileFullPath = Path.Combine(gameStateIntegrationPath, ConfigFile);
        return File.Exists(configFileFullPath);
    }

    string? FindDota2InstallationPath()
    {
        var libraryFoldersPath = GetLibraryFoldersPath();

        if (string.IsNullOrEmpty(libraryFoldersPath) || !File.Exists(libraryFoldersPath))
        {
            logger.LogError("libraryfolders.vdf not found");
            return null;
        }

        var steamInstallPath = GetSteamInstallationPath();

        if (string.IsNullOrEmpty(steamInstallPath))
        {
            logger.LogError("Steam installation path not found");
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

    string? GetSteamInstallationPath() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? GetWindowsInstall() :
        RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? GetMacInstall() : null;

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
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to read Steam installation path from registry");
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

    public void InstallIntegration()
    {
        var gameStateIntegrationPath = FindGameStateIntegrationPath();

        if (gameStateIntegrationPath == null)
        {
            logger.LogError("gamestate_integration folder not found");
            return;
        }

        var configFileFullPath = Path.Combine(gameStateIntegrationPath, ConfigFile);
        var configFileSourcePath = Path.Combine(Directory.GetCurrentDirectory(), ConfigFile);

        if (File.Exists(configFileFullPath)) File.Delete(configFileFullPath);

        File.Copy(configFileSourcePath, configFileFullPath);
    }

    [GeneratedRegex(@"\s*\""path\""\s*\""(.+?)\""", RegexOptions.Compiled)]
    private static partial Regex LibraryLocations();

    public void OpenGameStateIntegrationFolder()
    {
        var gameStateIntegrationPath = FindGameStateIntegrationPath();

        if (gameStateIntegrationPath == null)
        {
            logger.LogError("gamestate_integration folder not found");
            return;
        }

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