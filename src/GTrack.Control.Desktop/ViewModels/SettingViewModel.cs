using System.Reflection;
using System.Windows.Media;
using GTrack.Core.Models;
using GTrack.Core.Services;
using Microsoft.Extensions.Logging;
using Wpf.Ui.Appearance;

namespace GTrack.Control.Desktop.ViewModels;

/// <summary>
/// ViewModel for application settings, handles theme selection and app version
/// ViewModel для настроек приложения, отвечает за выбор темы и версию приложения
/// </summary>
public class SettingViewModel : BindableBase, INavigationAware
{
    private readonly ILogger<SettingViewModel> _logger;
    private readonly IConfigThemeService _configService;

    /// <summary>
    /// List of available application themes (Light and Dark)
    /// Список доступных тем приложения (Светлая и Темная)
    /// </summary>
    public IReadOnlyList<ApplicationTheme> Themes { get; } =
        new List<ApplicationTheme>
        {
            ApplicationTheme.Light,
            ApplicationTheme.Dark
        };

    private ApplicationTheme _currentApplicationTheme = ApplicationTheme.Unknown;
    /// <summary>
    /// Currently selected application theme
    /// Текущая выбранная тема приложения
    /// </summary>
    public ApplicationTheme CurrentApplicationTheme
    {
        get => _currentApplicationTheme;
        set
        {
            if (SetProperty(ref _currentApplicationTheme, value))
            {
                // Apply the selected theme immediately
                // Применяем выбранную тему сразу
                ApplicationThemeManager.Apply(value);

                // Save the theme selection to configuration
                // Сохраняем выбор темы в конфигурацию
                _configService.Save(new AppThemeConfig { Theme = value });
            }
        }
    }

    private string _appVersion = string.Empty;
    /// <summary>
    /// Application version string
    /// Строка с версией приложения
    /// </summary>
    public string AppVersion
    {
        get => _appVersion;
        set => SetProperty(ref _appVersion, value);
    }

    /// <summary>
    /// Constructor
    /// Конструктор SettingViewModel
    /// </summary>
    public SettingViewModel(ILogger<SettingViewModel> logger, IConfigThemeService configService)
    {
        _logger = logger;
        _configService = configService;

        // Initialize ViewModel properties
        // Инициализация свойств ViewModel
        InitializeViewModel();
    }

    /// <summary>
    /// Initializes theme and application version
    /// Инициализация темы и версии приложения
    /// </summary>
    private void InitializeViewModel()
    {
        // Load current application theme
        // Загружаем текущую тему приложения
        CurrentApplicationTheme = ApplicationThemeManager.GetAppTheme();

        // Get application version from assembly
        // Получаем версию приложения из сборки
        AppVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;

        // Subscribe to theme changes (e.g., system theme changes)
        // Подписка на изменения темы (например, при смене системной темы)
        ApplicationThemeManager.Changed += OnThemeChanged;
    }

    /// <summary>
    /// Event handler for when the application theme changes
    /// Обработчик события изменения темы приложения
    /// </summary>
    private void OnThemeChanged(ApplicationTheme currentApplicationTheme, Color systemAccent)
    {
        if (CurrentApplicationTheme != currentApplicationTheme)
        {
            CurrentApplicationTheme = currentApplicationTheme;
        }
    }

    // NavigationAware implementation
    // Реализация INavigationAware
    public void OnNavigatedTo(NavigationContext navigationContext) { }
    public bool IsNavigationTarget(NavigationContext navigationContext) => true;
    public void OnNavigatedFrom(NavigationContext navigationContext) { }
}