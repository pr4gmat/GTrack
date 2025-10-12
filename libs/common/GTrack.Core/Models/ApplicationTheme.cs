using Wpf.Ui.Appearance;

namespace GTrack.Core.Models;

/// <summary>
/// Represents the application theme configuration.
/// Stores the selected <see cref="ApplicationTheme"/>.
/// </summary>
///
/// <summary>
/// Представляет конфигурацию темы приложения.
/// Хранит выбранную тему <see cref="ApplicationTheme"/>.
/// </summary>
public class AppThemeConfig
{
    public ApplicationTheme Theme { get; set; } = ApplicationTheme.Dark;
}