using System.IO;
using System.Text.Json;
using GTrack.Core.Models;
using GTrack.Core.Services;

namespace GTrack.Infrastructure.Services;

/// <summary>
/// Service for loading and saving the application's theme configuration.
/// </summary>
/// <summary>
/// Сервис для загрузки и сохранения конфигурации темы приложения.
/// </summary>
public class ConfigThemeService : IConfigThemeService
{
    private readonly string _configPath;

    public ConfigThemeService(string appName)
    {
        if (string.IsNullOrWhiteSpace(appName))
            throw new ArgumentException("App name cannot be empty", nameof(appName));

        _configPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            appName,
            $"config_{appName}_theme.json");
    }

    public AppThemeConfig Load()
    {
        if (File.Exists(_configPath))
        {
            var json = File.ReadAllText(_configPath);
            return JsonSerializer.Deserialize<AppThemeConfig>(json) ?? new AppThemeConfig();
        }

        return new AppThemeConfig();
    }

    public void Save(AppThemeConfig themeConfig)
    {
        var dir = Path.GetDirectoryName(_configPath)!;
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var json = JsonSerializer.Serialize(themeConfig, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_configPath, json);
    }
}