using GTrack.Core.Models;

namespace GTrack.Core.Services;

/// <summary>
/// Service interface for loading and saving application theme configurations.
/// </summary>
///
/// <summary>
/// Интерфейс сервиса для загрузки и сохранения конфигураций темы приложения.
/// </summary>
public interface IConfigThemeService
{
    /// <summary>
    /// Loads the current application theme configuration.
    /// </summary>
    ///
    /// <summary>
    /// Загружает текущую конфигурацию темы приложения.
    /// </summary>
    /// <returns>An <see cref="AppThemeConfig"/> object representing the current theme.</returns>
    /// <returns>Объект <see cref="AppThemeConfig"/>, представляющий текущую тему.</returns>
    AppThemeConfig Load();

    /// <summary>
    /// Saves the provided application theme configuration.
    /// </summary>
    ///
    /// <summary>
    /// Сохраняет указанную конфигурацию темы приложения.
    /// </summary>
    /// <param name="themeConfig">The theme configuration to save.</param>
    /// <param name="themeConfig">Конфигурация темы для сохранения.</param>
    void Save(AppThemeConfig themeConfig);
}