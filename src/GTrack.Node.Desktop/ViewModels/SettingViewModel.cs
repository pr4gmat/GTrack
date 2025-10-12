using System.Reflection;
using System.Windows.Media;
using GTrack.Core.Models;
using GTrack.Core.Services;
using Microsoft.Extensions.Logging;
using Wpf.Ui.Appearance;

namespace GTrack.Node.Desktop.ViewModels;

/// <summary>
/// ViewModel for application settings in Node Desktop.
/// Manages theme selection and application version.
/// ViewModel для настроек приложения в Node Desktop.
/// Управляет выбором темы и версией приложения.
/// </summary>
public class SettingViewModel : BindableBase, INavigationAware
{
    private readonly ILogger<SettingViewModel> _logger;
    private readonly IConfigThemeService _configService;
    
    /// <summary>
    /// List of available themes (Light and Dark)
    /// Список доступных тем (Светлая и Тёмная)
    /// </summary>
    public IReadOnlyList<ApplicationTheme> Themes { get; } =
        new List<ApplicationTheme>
        {
            ApplicationTheme.Light,
            ApplicationTheme.Dark
        };
    
    /// <summary>
    /// Currently selected application theme
    /// Текущая выбранная тема приложения
    /// </summary>
    private ApplicationTheme _currentApplicationTheme = ApplicationTheme.Unknown;
    public ApplicationTheme CurrentApplicationTheme
    {
        get => _currentApplicationTheme;
        set
        {
            if (SetProperty(ref _currentApplicationTheme, value))
            {
                // Apply the selected theme to the application
                // Применяем выбранную тему к приложению
                ApplicationThemeManager.Apply(value);

                // Save the selected theme in configuration
                // Сохраняем выбранную тему в конфигурации
                _configService.Save(new AppThemeConfig { Theme = value });
            }
        }
    }
    
    /// <summary>
    /// Application version displayed in settings
    /// Версия приложения, отображаемая в настройках
    /// </summary>
    private string _appVersion = string.Empty;
    public string AppVersion
    {
        get => _appVersion;
        set => SetProperty(ref _appVersion, value);
    }
    
    /// <summary>
    /// Constructor initializes logger and configuration service
    /// Конструктор инициализирует логгер и сервис конфигурации
    /// </summary>
    public SettingViewModel(ILogger<SettingViewModel> logger, IConfigThemeService configService)
    {
        _logger = logger;
        _configService = configService;
        
        InitializeViewModel();
    }
    
    /// <summary>
    /// Initializes the ViewModel by loading theme and version
    /// Инициализация ViewModel: загрузка темы и версии приложения
    /// </summary>
    private void InitializeViewModel()
    {
        CurrentApplicationTheme = ApplicationThemeManager.GetAppTheme();
        AppVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;

        // Subscribe to theme changes in real-time
        // Подписка на изменения темы в реальном времени
        ApplicationThemeManager.Changed += OnThemeChanged;
    }
    
    /// <summary>
    /// Handles theme change events and updates CurrentApplicationTheme
    /// Обрабатывает события изменения темы и обновляет CurrentApplicationTheme
    /// </summary>
    private void OnThemeChanged(ApplicationTheme currentApplicationTheme, Color systemAccent)
    {
        if (CurrentApplicationTheme != currentApplicationTheme)
        {
            CurrentApplicationTheme = currentApplicationTheme;
        }
    }
    
    // Navigation-aware interface methods
    // Методы интерфейса для навигации
    public void OnNavigatedTo(NavigationContext navigationContext) { }
    public bool IsNavigationTarget(NavigationContext navigationContext) => true;
    public void OnNavigatedFrom(NavigationContext navigationContext) { }
}