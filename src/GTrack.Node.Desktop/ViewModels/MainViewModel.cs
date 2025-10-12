using System.Windows;
using GTrack.Core.Services;
using GTrack.Node.Desktop.Views;
using Microsoft.Extensions.Logging;
using Wpf.Ui.Appearance;

namespace GTrack.Node.Desktop.ViewModels;

/// <summary>
/// Main ViewModel for the Node desktop application.
/// Главная ViewModel для десктопного приложения Node.
/// Manages the window title, title bar, and splash screen navigation.
/// Управляет заголовком окна, панелью заголовка и навигацией splash screen.
/// </summary>
public class MainViewModel : BindableBase
{
    private readonly IRegionManager _regionManager;
    private readonly ILogger<MainViewModel> _logger;
    private readonly IConfigThemeService _configService;
    
    // Window title text
    // Текст заголовка окна
    private string _titleText;
    public string TitleText
    {
        get => _titleText;
        set => SetProperty(ref _titleText, value);
    }
    
    // Margin around the title bar
    // Отступы вокруг панели заголовка
    private Thickness _titleBarMargin;
    public Thickness TitleBarMargin
    {
        get => _titleBarMargin;
        set => SetProperty(ref _titleBarMargin, value);
    }
    
    // Height of the title bar
    // Высота панели заголовка
    private double _titleBarHeight;
    public double TitleBarHeight
    {
        get => _titleBarHeight;
        set => SetProperty(ref _titleBarHeight, value);
    }
    
    /// <summary>
    /// Constructor initializes services and default UI values.
    /// Конструктор инициализирует сервисы и значения по умолчанию для UI.
    /// </summary>
    public MainViewModel(IRegionManager regionManager, ILogger<MainViewModel> logger,
        IConfigThemeService configService)
    {
        _regionManager = regionManager;
        _logger = logger;
        _configService = configService;
        
        // Set initial title and margins
        // Устанавливаем начальный заголовок и отступы
        TitleText = "GTrack Node Desktop";
        TitleBarMargin = new Thickness(5, 5, 5, 0);
        TitleBarHeight = 0;
        
        InitializeApplicationAsync();
    }
    
    /// <summary>
    /// Initializes the application asynchronously: loads theme and navigates to the splash screen.
    /// Асинхронная инициализация приложения: загрузка темы и переход на splash screen.
    /// </summary>
    private async void InitializeApplicationAsync()
    {
        LoadTheme(); // Apply the current theme // Применяем текущую тему
            
        await Task.Delay(3000); // Simulate splash screen delay // Симуляция задержки splash screen

        TitleBarHeight = 30; // Set title bar height after splash // Устанавливаем высоту панели заголовка после splash
        
        // Navigate to the main workspace view
        // Переход на основное рабочее пространство
        var splashScreenRegion = _regionManager.Regions["MainRegion"];
        _regionManager.RequestNavigate("MainRegion", nameof(WorkspaceView));
    }
    
    /// <summary>
    /// Loads and applies the saved application theme.
    /// Загружает и применяет сохранённую тему приложения.
    /// </summary>
    private void LoadTheme()
    {
        var config = _configService.Load();
        ApplicationThemeManager.Apply(config.Theme);
    }
}