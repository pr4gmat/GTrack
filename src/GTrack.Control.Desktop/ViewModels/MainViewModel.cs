using System.Windows;
using GTrack.Control.Desktop.Views;
using GTrack.Core.Services;
using Microsoft.Extensions.Logging;
using Wpf.Ui.Appearance;

namespace GTrack.Control.Desktop.ViewModels;

/// <summary>
/// Main application ViewModel
/// Главная ViewModel приложения, отвечает за стартовое окно и темы
/// </summary>
public class MainViewModel : BindableBase
{
    private readonly IRegionManager _regionManager;
    private readonly ILogger<MainViewModel> _logger;
    private readonly IConfigThemeService _configService;

    private string _titleText;
    /// <summary>
    /// Text displayed in the application title bar
    /// Текст, отображаемый в заголовке окна приложения
    /// </summary>
    public string TitleText
    {
        get => _titleText;
        set => SetProperty(ref _titleText, value);
    }

    private Thickness _titleBarMargin;
    /// <summary>
    /// Margin for the title bar
    /// Отступы заголовка окна
    /// </summary>
    public Thickness TitleBarMargin
    {
        get => _titleBarMargin;
        set => SetProperty(ref _titleBarMargin, value);
    }

    private double _titleBarHeight;
    /// <summary>
    /// Height of the title bar
    /// Высота заголовка окна
    /// </summary>
    public double TitleBarHeight
    {
        get => _titleBarHeight;
        set => SetProperty(ref _titleBarHeight, value);
    }

    /// <summary>
    /// Constructor
    /// Конструктор MainViewModel
    /// </summary>
    public MainViewModel(IRegionManager regionManager, ILogger<MainViewModel> logger,
        IConfigThemeService configService)
    {
        _regionManager = regionManager;
        _logger = logger;
        _configService = configService;

        TitleText = "GTrack Control Desktop";
        TitleBarMargin = new Thickness(5, 5, 5, 0);
        TitleBarHeight = 0;

        // Initialize application asynchronously
        // Асинхронная инициализация приложения
        InitializeApplicationAsync();
    }

    /// <summary>
    /// Initializes the application: loads theme, waits, and navigates to main workspace
    /// Инициализация приложения: загрузка темы, ожидание и навигация к рабочему пространству
    /// </summary>
    private async void InitializeApplicationAsync()
    {
        LoadTheme();

        // Wait for splash screen or startup animation
        // Ждем, пока splash screen или анимация запуска завершится
        await Task.Delay(3000);

        // Set title bar height after initialization
        // Устанавливаем высоту заголовка после инициализации
        TitleBarHeight = 30;

        // Navigate to main workspace view
        // Навигация к главному рабочему пространству
        var splashScreenRegion = _regionManager.Regions["MainRegion"];
        _regionManager.RequestNavigate("MainRegion", nameof(WorkspaceView));
    }

    /// <summary>
    /// Loads the saved application theme from configuration and applies it
    /// Загружает сохраненную тему приложения из конфигурации и применяет её
    /// </summary>
    private void LoadTheme()
    {
        var config = _configService.Load();
        ApplicationThemeManager.Apply(config.Theme);
    }
}